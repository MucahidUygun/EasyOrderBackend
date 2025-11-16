using Core.Entities;
using Core.Security.Enums;
using Core.Security.JWT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Contracts.Security.Interfaces;

public interface IRefreshTokenService
{
    public AccessToken CreateAccessToken(BaseUser user, List<BaseClaim> baseClaims);
    public Task<BaseRefreshToken> CreateRefreshToken(BaseUser user, string ipAdress);
    public Task<BaseRefreshToken?> GetRefreshTokenByToken(string refreshToken,string ipAdress);
    public Task<BaseRefreshToken> AddRefreshToken(BaseRefreshToken refreshToken);
    public Task DeleteOldRefreshToken(BaseUser user, string ipAdress);
    public Task RevokeDescendantRefreshTokens(BaseRefreshToken refreshToken, string ipAddress, string reason);

    public Task RevokeRefreshToken(BaseRefreshToken token, string ipAddress, string? reason = null, string? replacedByToken = null);

    public Task<BaseRefreshToken> RotateRefreshToken(BaseUser user, string refreshToken, string ipAddress);
    public Task<RefreshTokenValidType> GetRefreshTokenValidType(string refreshToken,string createdByIp,BaseUser user);
}
