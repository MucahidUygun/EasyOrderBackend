using Core.Entities;
using Core.Security.JWT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Contracts.Security.Interfaces;

public interface IHttpService
{
    public string? GetAccessTokenFromHeaders();
    public string? GetRefreshTokenFromCookie();
    public string? GetByIpAdressFromHeaders();
    public void SetAccessTokenAndRefreshTokenFromRequest(BaseUser user, List<BaseClaim> baseClaims, AccessToken newAccessToken, BaseRefreshToken refreshToken);
}
