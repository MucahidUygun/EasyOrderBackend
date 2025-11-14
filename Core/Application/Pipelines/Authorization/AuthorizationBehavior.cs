using Core.Application.Contracts.Security.Interfaces;
using Core.CrossCuttingConcerns.Expeptions.Types;
using Core.Entities;
using Core.Security.Enums;
using Core.Security.Extensions;
using Core.Security.JWT;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Pipelines.Authorization;

public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ISecuredRequest
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITokenHelper _tokenHelper;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly HttpContext _httpContext;
    public AuthorizationBehavior(IHttpContextAccessor httpContextAccessor, ITokenHelper tokenHelper,
    IRefreshTokenService refreshTokenService)
    {
        _tokenHelper = tokenHelper;
        _httpContextAccessor = httpContextAccessor;
        _refreshTokenService = refreshTokenService;
        _httpContext = _httpContextAccessor.HttpContext;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        //Kullanıcı authenticated olduğunu kontrol ediyor
        if (!_httpContext.User.Claims.Any())
        {
            string? oldAccessToken = _httpContext
                ?.Request
                ?.Headers["Authorization"]
                 .ToString()
                 .Replace("Bearer ", "");

            string refreshTokenFromCookie = _httpContext!.Request.Cookies.FirstOrDefault(p => p.Key.Equals("refreshToken")).Value;
            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtSecurityToken = jwtSecurityTokenHandler.ReadJwtToken(oldAccessToken);

            Guid id = Guid.Parse(input: jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!);
            string? createdByIp = _httpContext!.Request.Headers.ContainsKey("X-Forwarded-For")
            ? _httpContext!.Request.Headers["X-Forwarded-For"].ToString()
            : _httpContext!.Connection.RemoteIpAddress?.MapToIPv4().ToString();

            if (createdByIp is null) throw new AuthorizationException("You are not authorized.");

            BaseUser user = new()
            {
                Id = id,
                Email = jwtSecurityToken.Claims.FirstOrDefault(c =>
                    c.Type == JwtRegisteredClaimNames.Email)?.Value,

            };
            List<BaseClaim> baseClaims = jwtSecurityToken.Claims
                .Where(c => c.Type == "Role")   // 🔹 Burada dikkat
                .Select(c => new BaseClaim
                {
                    Name = c.Value  // claim value: örn. "Admin"
                })
                .ToList();

            RefreshTokenValidType validType = await _refreshTokenService.GetRefreshTokenValidType(refreshTokenFromCookie,createdByIp!,user);

            switch (validType)
            {
                case RefreshTokenValidType.Active:
                    BaseRefreshToken createdRefreshToken = await _refreshTokenService.RotateRefreshToken(user,refreshTokenFromCookie,createdByIp);
                    AccessToken accessToken = CreateAccessToken(user, baseClaims);
                    SetAccessTokenAndRefreshTokenFromRequest(user,baseClaims,accessToken,createdRefreshToken);
                    break;
                case RefreshTokenValidType.Expired:
                    BaseRefreshToken? revokeRefreshToken = await _refreshTokenService.GetRefreshTokenByToken(refreshTokenFromCookie, createdByIp);
                    await _refreshTokenService.RevokeRefreshToken(revokeRefreshToken!,createdByIp,reason:"Token expired.");
                    throw new AuthorizationException("Please log in");
                case RefreshTokenValidType.Deleted:
                    BaseRefreshToken? revokeAllRefreshToken = await _refreshTokenService.GetRefreshTokenByToken(refreshTokenFromCookie, createdByIp);
                    await _refreshTokenService.RevokeDescendantRefreshTokens(revokeAllRefreshToken!,createdByIp, "Token reuse detected");
                    break;
                case RefreshTokenValidType.NotFound:
                    throw new AuthorizationException("You are not authorized.");
            }
        }
        if (request.Claims.Any())
        {   //Tokendan kullanıcı role bilgileri alınıyor ICollection burada ki kritik yerlerden!
            ICollection<string>? userRoleClaims = _httpContext.User.ClaimRoles() ?? [];
            //Burada tokendan alınınan role bilgileri ile request(classının veya yetki gereken yer) ile roller eşleşme durumuna bakılıyor.
            bool isNotMatchedAUserRoleClaimWithRequestRoles = userRoleClaims
                .FirstOrDefault(userRoleClaim =>
                    userRoleClaim == "Admin" || request.Claims.Contains(userRoleClaim)
                )
                == null;
            if (isNotMatchedAUserRoleClaimWithRequestRoles)
                throw new AuthorizationException("You are not authorized.");


        }
        TResponse response = await next();
        return response;
    }

    private AccessToken CreateAccessToken(BaseUser user, List<BaseClaim> baseClaims)
    {
        AccessToken newAccessToken = _tokenHelper.CreateToken(user: user, baseClaims);
        return newAccessToken;
    }

    private void SetAccessTokenAndRefreshTokenFromRequest(BaseUser user, List<BaseClaim> baseClaims, AccessToken newAccessToken,BaseRefreshToken refreshToken)
    {
        ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                new[]
                {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? "")
                }.Concat(baseClaims.Select(c => new Claim("Role", c.Name))),
                "Custom");
        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        _httpContext!.User = claimsPrincipal;
        _httpContext.Response.Headers["Authorization"] = $"Bearer {newAccessToken.Token}";

        CookieOptions cookieOptions = new()
        {
            HttpOnly = true,           // JS erişemez
            Secure = true,             // sadece HTTPS üzerinden
            SameSite = SameSiteMode.Strict, // CSRF'ye karşı koruma
            Expires = refreshToken.ExpiresDate
        };
        _httpContext.Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
    }
}
