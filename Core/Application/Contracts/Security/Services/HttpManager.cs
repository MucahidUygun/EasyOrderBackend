using Core.Application.Contracts.Security.Interfaces;
using Core.Entities;
using Core.Security.JWT;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Contracts.Security.Services;

public class HttpManager : IHttpService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly HttpContext _httpContext;

    public HttpManager(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _httpContext = _httpContextAccessor.HttpContext;
    }

    public string? GetAccessTokenFromHeaders()
    {
        return _httpContext
        ?.Request
        ?.Headers["Authorization"]
                 .ToString()
                 .Replace("Bearer ", "");
    }

    public string? GetByIpAdressFromHeaders()
    {
        return _httpContext!.Request.Headers.ContainsKey("X-Forwarded-For")
        ? _httpContext!.Request.Headers["X-Forwarded-For"].ToString()
        : _httpContext!.Connection.RemoteIpAddress?.MapToIPv4().ToString();
    }

    public string? GetRefreshTokenFromCookie()
    {
        return _httpContext!.Request.Cookies.FirstOrDefault(p => p.Key.Equals("refreshToken")).Value;
    }

    public void SetAccessTokenAndRefreshTokenFromRequest(BaseUser user, List<BaseClaim> baseClaims, AccessToken newAccessToken, BaseRefreshToken refreshToken)
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
