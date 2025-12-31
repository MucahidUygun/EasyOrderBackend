using Core.Application.Contracts.Security.Interfaces;
using Core.Constants;
using Core.CrossCuttingConcerns.Expeptions.Types;
using Core.Entities;
using Core.Security.Enums;
using Core.Security.JWT;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver.Authentication.Oidc;
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
    private readonly IHttpService _httpService;
    private readonly IJwtService _jwtService;

    public RefreshTokenManager(IRefreshTokenRepository refreshTokenRepository, ITokenHelper tokenHelper, IHttpService httpService, IJwtService jwtService)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _tokenHelper = tokenHelper;
        _httpService = httpService;
        _jwtService = jwtService;
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
        if (_httpService.GetDeviceIdAdressFromHeaders() is null
          && _httpService.GetUserAgentFromHeaders() is null
          && _httpService.GetDevicePlatformFromHeaders() is null
          )
            throw new AuthorizationException("You are not auth");

        BaseRefreshToken baseRefreshToken = _tokenHelper.CreateRefreshToken(
            user: user,
            ipAdress: ipAdress,
            deviceId: _httpService.GetDeviceIdAdressFromHeaders()!,
            userAgent: _httpService.GetUserAgentFromHeaders()!,
            devicePlatform: _httpService.GetDevicePlatformFromHeaders()!,
            deviceName: _httpService.GetDeviceNameFromHeaders());

        return await Task.FromResult(baseRefreshToken);
    }

    public async Task DeleteOldRefreshToken(BaseUser user, string ipAdress)
    {
        IEnumerable<BaseRefreshToken> tokens = await _refreshTokenRepository.GetOldRefreshTokensAsync(user, ipAdress);

        // burada list'e çevir
        List<BaseRefreshToken> tokenList = tokens.ToList();


        //List<BaseRefreshToken> refreshTokens = await _refreshTokenRepository.GetOldRefreshTokensAsync(user, ipAdress);
        await _refreshTokenRepository.DeleteRangeAsync(tokenList);
    }

    public async Task<BaseRefreshToken?> GetRefreshTokenByToken(string refreshToken,string ipAdress,bool withDeleted=false)
    {
        BaseRefreshToken? token = await _refreshTokenRepository.GetAsync(p => p.Token == refreshToken && p.CreatedByIp == ipAdress,withDeleted:withDeleted);
        return token;
    }

    public async Task<RefreshTokenValidType> GetRefreshTokenValidType(string refreshToken, string createdByIp,BaseUser user)
    {
        BaseRefreshToken? baseRefreshToken = await GetRefreshTokenByToken(refreshToken,createdByIp,true);

        if (baseRefreshToken is null) return RefreshTokenValidType.NotFound;
        if (baseRefreshToken.ExpiresDate >= DateTime.UtcNow) return RefreshTokenValidType.Active;
        if (baseRefreshToken.RevokedDate <= DateTime.Now || baseRefreshToken.Token is null) return RefreshTokenValidType.Expired;
        else return RefreshTokenValidType.Deleted;
    }

    public async Task RefreshAsync(string refreshToken, string ip,string oldAccessToken)
    {
        Guid id = Guid.Parse(input: _jwtService.GetIdFromOldAccesToken(oldAccessToken));

        BaseUser user = new()
        {
            Id = id,
            Email = _jwtService.GetEmailFromOldAccesToken(oldAccessToken),

        };
        List<BaseClaim> baseClaims = _jwtService.GetClaimsByKey(oldAccessToken, CoreMessages.CliamRole);

        RefreshTokenValidType validType = await GetRefreshTokenValidType(refreshToken, ip!, user);

        switch (validType)
        {
            case RefreshTokenValidType.Active:
                BaseRefreshToken createdRefreshToken = await RotateRefreshToken(user, refreshToken, ip);
                AccessToken accessToken = CreateAccessToken(user, baseClaims);
                _httpService.SetAccessTokenAndRefreshTokenFromRequest(user, baseClaims, accessToken, createdRefreshToken);
                break;
            case RefreshTokenValidType.Expired:
                BaseRefreshToken? revokeRefreshToken = await GetRefreshTokenByToken(refreshToken, ip);
                await RevokeRefreshToken(revokeRefreshToken!, ip, reason: CoreMessages.TokenExpired);
                throw new AuthorizationException(CoreMessages.LogIn);
            case RefreshTokenValidType.Deleted:
                BaseRefreshToken? revokeAllRefreshToken = await GetRefreshTokenByToken(refreshToken, ip);
                await RevokeDescendantRefreshTokens(revokeAllRefreshToken!, ip, CoreMessages.TokenReuseDetected);
                break;
            case RefreshTokenValidType.NotFound:
                throw new AuthorizationException(CoreMessages.NotAuthorized);
        }
    }

    //RefreshToken zincirinde ki aktif olan hariç hepsinin iptali 
    public async Task RevokeDescendantRefreshTokens(BaseRefreshToken refreshToken, string ipAddress, string reason)
    {
        BaseRefreshToken? childToken = await _refreshTokenRepository.GetAsync(predicate: r =>
             r.Token == refreshToken.ReplacedByToken
         );
        if (childToken is null || childToken?.RevokedDate is not null)
        {
            await RevokeRefreshToken(refreshToken, ipAddress, reason);
            return;
        }
        if (childToken?.ExpiresDate <= DateTime.UtcNow)
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
        token.DeletedDate = DateTime.UtcNow;
        await _refreshTokenRepository.UpdateAsync(token);
    }
    //Üretilen RefreshTokenı eski RefreshTokena bağlama
    public async Task<BaseRefreshToken> RotateRefreshToken(BaseUser user, string refreshToken, string ipAdress)
    {
        if (_httpService.GetDeviceIdAdressFromHeaders() is null
           && _httpService.GetUserAgentFromHeaders() is null
           && _httpService.GetDevicePlatformFromHeaders() is null
           )
            throw new AuthorizationException("You are not auth");

        BaseRefreshToken? hasRefreshToken = await GetRefreshTokenByToken(refreshToken,ipAdress);

        BaseRefreshToken baseRefreshToken = _tokenHelper.CreateRefreshToken(
            user: user,
            ipAdress: ipAdress,
            deviceId: _httpService.GetDeviceIdAdressFromHeaders()!,
            userAgent: _httpService.GetUserAgentFromHeaders()!,
            devicePlatform: _httpService.GetDevicePlatformFromHeaders()!,
            deviceName: _httpService.GetDeviceNameFromHeaders());
        await RevokeRefreshToken(hasRefreshToken!, ipAdress, reason: CoreMessages.ReplacedByNewToken, baseRefreshToken.Token);
        await AddRefreshToken(baseRefreshToken);

        return baseRefreshToken;
    }
}
