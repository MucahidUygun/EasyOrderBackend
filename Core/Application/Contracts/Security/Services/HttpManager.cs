using Core.Application.Contracts.Security.Interfaces;
using Core.Constants;
using Core.Entities;
using Core.Security.JWT;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using static System.Net.WebRequestMethods;

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

    public void DeleteRefreshTokenFromCookie()
    {
        _httpContextAccessor.HttpContext!.Response.Cookies.Delete("refreshToken");
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

    public string? GetDeviceIdAdressFromHeaders()
    {
        return _httpContext!.Request.Headers.ContainsKey("X-Device-Id")
        ? _httpContext!.Request.Headers["X-Device-Id"].ToString()
        : null;
    }

    public string? GetDeviceNameFromHeaders()
    {
        return _httpContext!.Request.Headers.ContainsKey("X-Device-Name")
        ? _httpContext!.Request.Headers["X-Device-Name"].ToString()
        : null;
    }

    public string? GetDevicePlatformFromHeaders()
    {
        return _httpContext!.Request.Headers.ContainsKey("X-Platform")
        ? _httpContext!.Request.Headers["X-Platform"].ToString()
        : null;
    }

    public string? GetRefreshTokenFromCookie()
    {
        return _httpContext!.Request.Cookies.FirstOrDefault(p => p.Key.Equals("refreshToken")).Value;
    }

    public string? GetUserAgentFromHeaders()
    {
        return _httpContext!.Request.Headers["User-Agent"];
    }

    public void SetAccessTokenAndRefreshTokenFromRequest(BaseUser user, List<BaseClaim> baseClaims, AccessToken newAccessToken, BaseRefreshToken refreshToken)
    {
        ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                new[]
                {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? "")
                }.Concat(baseClaims.Select(c => new Claim(CoreMessages.CliamRole, c.Name))),
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
