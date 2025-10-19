using AutoMapper;
using Core.Entities;
using Core.Security.JWT;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Persistence.Services;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.AuthService;

public class AuthManager : IAuthService
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenHelper _tokenHelper;
    private readonly IUserOperationClaimRepository _userOperationClaimRepository;
    private readonly TokenOptions _tokenOptions;
    private readonly IMapper _mapper;

    public AuthManager(
    IRefreshTokenRepository refreshTokenRepository,
    ITokenHelper tokenHelper,
    IMapper mapper,
    IConfiguration configuration,
    IUserOperationClaimRepository userOperationClaimRepository
)
    {
        _userOperationClaimRepository = userOperationClaimRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _tokenHelper = tokenHelper;
        _mapper = mapper;
        const string tokenOptionsConfigurationSection = "TokenOptions";
        _tokenOptions =
            configuration.GetSection(tokenOptionsConfigurationSection).Get<TokenOptions>()
            ?? throw new NullReferenceException($"\"{tokenOptionsConfigurationSection}\" section cannot found in configuration");
    }


    public async Task<RefreshToken> AddRefreshToken(RefreshToken refreshToken)
    {
        RefreshToken addedRefreshToken = await _refreshTokenRepository.AddAsync(refreshToken); 

        return addedRefreshToken;
    }

    public async Task<AccessToken> CreateAccessToken(User user)
    {
        IList<OperationClaim> operationClaims = await _userOperationClaimRepository.GetOperationClaimsByUserIdAsync(user.Id);

        AccessToken accessToken = _tokenHelper.CreateToken(
            user,
            operationClaims.Select(p=>(BaseClaim)p).ToImmutableList()
            );

        return accessToken;
    }

    public Task<RefreshToken> CreateRefreshToken(User user, string ipAdress)
    {
        BaseRefreshToken coreRefreshToken = _tokenHelper.CreateRefreshToken(user,ipAdress);

        RefreshToken refreshToken = _mapper.Map<RefreshToken>(coreRefreshToken);

        return Task.FromResult(refreshToken);
    }

    public async Task DeleteOldRefreshToken(Guid id)
    {
        List<RefreshToken> refreshTokens = await _refreshTokenRepository.GetOldRefreshTokensAsync(userId:id,refreshTokenTTL:_tokenOptions.RefreshTokenTTL);
        await _refreshTokenRepository.DeleteRangeAsync(refreshTokens);
    }

    public async Task<RefreshToken?> GetRefreshTokenByToken(string refreshToken)
    {
        RefreshToken? token = await _refreshTokenRepository.GetAsync(p=>p.Token==refreshToken);  
        return token;
    }

    public async Task RevokeDescendantRefreshTokens(RefreshToken refreshToken, string ipAddress, string reason)
    {
        RefreshToken? childToken = await _refreshTokenRepository.GetAsync(predicate: r =>
            r.Token == refreshToken.ReplacedByToken
        );

        if (childToken?.Revoked != null && childToken.Expires <= DateTime.UtcNow)
            await RevokeRefreshToken(childToken, ipAddress, reason);
        else
            await RevokeDescendantRefreshTokens(refreshToken: childToken!, ipAddress, reason);
    }

    public async Task RevokeRefreshToken(RefreshToken token, string ipAddress, string? reason = null, string? replacedByToken = null)
    {
        token.Revoked = DateTime.UtcNow;
        token.RevokedByIp = ipAddress;
        token.ReasonRevoked = reason;
        token.ReplacedByToken = replacedByToken;
        await _refreshTokenRepository.UpdateAsync(token);
    }

    public async Task<RefreshToken> RotateRefreshToken(User user, RefreshToken refreshToken, string ipAddress)
    {
        BaseRefreshToken baseRefreshToken = _tokenHelper.CreateRefreshToken(user,ipAddress);
        RefreshToken newRefreshToken = _mapper.Map<RefreshToken>(baseRefreshToken);
        await RevokeRefreshToken(refreshToken,ipAddress,reason: "Replaced by new token", newRefreshToken.Token);

        return newRefreshToken;
    }

}
