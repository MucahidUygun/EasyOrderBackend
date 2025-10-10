using Core.Security.JWT;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.AuthService;

public interface IAuthService
{
    public Task<AccessToken> CreateAccessToken(User user);
    public Task<RefreshToken> CreateRefreshToken(User user,string ipAdress);
    public Task<RefreshToken?> GetRefreshTokenByToken(string refreshToken);
    public Task<RefreshToken> AddRefreshToken(RefreshToken refreshToken);
    public Task DeleteOldRefreshToken(Guid id);
    public Task RevokeDescendantRefreshTokens(RefreshToken refreshToken, string ipAddress, string reason);

    public Task RevokeRefreshToken(RefreshToken token, string ipAddress, string? reason = null, string? replacedByToken = null);

    public Task<RefreshToken> RotateRefreshToken(User user, RefreshToken refreshToken, string ipAddress);

}
