using Core.Application.Contracts.Security.Interfaces;
using Core.CrossCuttingConcerns.Expeptions.Types;
using Core.Entities;
using Core.Security.Enums;
using Core.Security.Extensions;
using Core.Security.JWT;
using MediatR;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Pipelines.Authorization;

public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ISecuredRequest
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenHelper _tokenHelper;
    public AuthorizationBehavior(IHttpContextAccessor httpContextAccessor,IRefreshTokenRepository refreshTokenValidator,ITokenHelper tokenHelper)
    {
        _tokenHelper = tokenHelper;
        _refreshTokenRepository = refreshTokenValidator;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        //Kullanıcı authenticated olduğunu kontrol ediyor
        if (!_httpContextAccessor.HttpContext.User.Claims.Any())
        {
            string? oldAccessToken = _httpContextAccessor.HttpContext
                ?.Request
                ?.Headers["Authorization"]
                 .ToString()
                 .Replace("Bearer ", "");

            string refreshTokenFromCookie = _httpContextAccessor.HttpContext.Request.Cookies.FirstOrDefault(p => p.Key.Equals("refreshToken")).Value;

            RefreshTokenValidType validType = await _refreshTokenRepository.IsValidRefreshToken(refreshTokenFromCookie);

            //switch (validType)
            //{
            //    case RefreshTokenValidType.Active:
            //        await next();
            //        break;
            //    case RefreshTokenValidType.Expired:
            //        break;
            //    case RefreshTokenValidType.Deleted:
            //        break;
            //    case RefreshTokenValidType.NotFound:
            //        break;
            //    default:
            //        break;
            //}

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtSecurityToken = jwtSecurityTokenHandler.ReadJwtToken(oldAccessToken);

            Guid id = Guid.Parse(input: jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!);
            string? createdByIp = _httpContextAccessor.HttpContext!.Request.Headers["X-Forwarded-For"].FirstOrDefault();

            BaseRefreshToken? refreshToken = await _refreshTokenRepository.GetAsync(
                p => p.UserId == id && p.CreatedByIp == createdByIp,
                cancellationToken: cancellationToken);

            if (string.IsNullOrWhiteSpace(refreshToken?.Token))
                throw new AuthorizationException("Refresh token is required for this request.");

            if (refreshToken.Token != refreshTokenFromCookie || refreshToken == null)
                throw new AuthorizationException("You are not authorized.");


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
            CreateAccessToken(user, baseClaims);
        }
        if (request.Claims.Any())
        {   //Tokendan kullanıcı role bilgileri alınıyor ICollection burada ki kritik yerlerden!
            ICollection<string>? userRoleClaims = _httpContextAccessor.HttpContext.User.ClaimRoles() ?? [];
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

    private void CreateAccessToken(BaseUser user, List<BaseClaim> baseClaims)
    {
        AccessToken newAccessToken = _tokenHelper.CreateToken(user: user, baseClaims);

        ClaimsIdentity claimsIdentity = new ClaimsIdentity(
        new[]
        {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? "")
        }.Concat(baseClaims.Select(c => new Claim("Role", c.Name))),
        "Custom");
        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        _httpContextAccessor.HttpContext!.User = claimsPrincipal;
        _httpContextAccessor.HttpContext.Response.Headers["Authorization"] = $"Bearer {newAccessToken.Token}";
    }
}
