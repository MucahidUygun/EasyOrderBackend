using Core.Application.Contracts.Security.Interfaces;
using Core.Entities;
using Core.Security.Enums;
using Core.Security.JWT;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Contracts.Security.Services;

public class RefreshTokenManager : IRefreshTokenService
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenHelper _tokenHelper;
    private readonly TokenOptions _tokenOptions;

    public RefreshTokenManager(IRefreshTokenRepository refreshTokenRepository, ITokenHelper tokenHelper,IConfiguration configuration)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _tokenHelper = tokenHelper;
        const string tokenOptionsConfigurationSection = "TokenOptions";
        _tokenOptions =
            configuration.GetSection(tokenOptionsConfigurationSection).Get<TokenOptions>()
            ?? throw new NullReferenceException($"\"{tokenOptionsConfigurationSection}\" section cannot found in configuration");
    }

    public async Task<BaseRefreshToken> AddRefreshToken(BaseRefreshToken refreshToken)
    {
        BaseRefreshToken addedBaseRefreshToken = await _refreshTokenRepository.AddAsync(refreshToken);

        return addedBaseRefreshToken;
    }

    public AccessToken CreateAccessToken(BaseUser user, List<BaseClaim> baseClaims)
    {
        AccessToken newAccessToken = _tokenHelper.CreateToken(user: user, baseClaims);
        return newAccessToken;
    }

    public async Task<BaseRefreshToken> CreateRefreshToken(BaseUser user, string ipAdress)
    {
        BaseRefreshToken coreRefreshToken = _tokenHelper.CreateRefreshToken(user, ipAdress);

        return await Task.FromResult(coreRefreshToken);
    }

    public async Task DeleteOldRefreshToken(BaseUser user, string ipAdress)
    {
        List<BaseRefreshToken> refreshTokens = await _refreshTokenRepository.GetOldRefreshTokensAsync(user, ipAdress);
        await _refreshTokenRepository.DeleteRangeAsync(refreshTokens);
    }

    public async Task<BaseRefreshToken?> GetRefreshTokenByToken(string refreshToken,string ipAdress)
    {
        BaseRefreshToken? token = await _refreshTokenRepository.GetAsync(p => p.Token == refreshToken && p.CreatedByIp == ipAdress);
        return token;
    }

    public async Task<RefreshTokenValidType> GetRefreshTokenValidType(string refreshToken, string createdByIp,BaseUser user)
    {
        BaseRefreshToken? baseRefreshToken = await GetRefreshTokenByToken(refreshToken,createdByIp);

        if (baseRefreshToken is null) return RefreshTokenValidType.NotFound;
        if (baseRefreshToken.ExpiresDate >= DateTime.UtcNow) return RefreshTokenValidType.Active;
        if (baseRefreshToken.RevokedDate <= DateTime.Now || baseRefreshToken.Token is null) return RefreshTokenValidType.Expired;
        else return RefreshTokenValidType.Deleted;
    }

    //RefreshToken zincirinde ki aktif olan hariç hepsinin iptali 
    public async Task RevokeDescendantRefreshTokens(BaseRefreshToken refreshToken, string ipAddress, string reason)
    {
        BaseRefreshToken? childToken = await _refreshTokenRepository.GetAsync(predicate: r =>
            r.Token == refreshToken.ReplacedByToken
        );

        if (childToken?.RevokedDate != null && childToken.ExpiresDate <= DateTime.UtcNow)
            await RevokeRefreshToken(childToken, ipAddress, reason);
        else
            await RevokeDescendantRefreshTokens(refreshToken: childToken!, ipAddress, reason);
    }
    // RefreshTokenın iptali
    public async Task RevokeRefreshToken(BaseRefreshToken token, string ipAddress, string? reason = null, string? replacedByToken = null)
    {
        token.RevokedDate = DateTime.UtcNow;
        token.RevokedByIp = ipAddress;
        token.ReasonRevoked = reason;
        token.ReplacedByToken = replacedByToken;
        token.IsActive = false;
        await _refreshTokenRepository.UpdateAsync(token);
    }
    //Üretilen RefreshTokenı eski RefreshTokena bağlama
    public async Task<BaseRefreshToken> RotateRefreshToken(BaseUser user, string refreshToken, string ipAddress)
    {
        BaseRefreshToken? hasRefreshToken = await GetRefreshTokenByToken(refreshToken,ipAddress);
        BaseRefreshToken baseRefreshToken = _tokenHelper.CreateRefreshToken(user, ipAddress);
        await RevokeRefreshToken(hasRefreshToken!, ipAddress, reason: "Replaced by new token", baseRefreshToken.Token);
        await AddRefreshToken(baseRefreshToken);

        return baseRefreshToken;
    }
}
