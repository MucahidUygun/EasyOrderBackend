using AutoMapper;
using Core.Application.Contracts.Security.Interfaces;
using Core.Entities;
using Core.Security.JWT;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Configuration;
using Persistence.Services;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
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
    private readonly IUserRepository _userRepository;

    public AuthManager(
    IRefreshTokenRepository refreshTokenRepository,
    ITokenHelper tokenHelper,
    IMapper mapper,
    IConfiguration configuration,
    IUserOperationClaimRepository userOperationClaimRepository,
    IUserRepository userRepository
)
    {
        _userRepository = userRepository;
        _userOperationClaimRepository = userOperationClaimRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _tokenHelper = tokenHelper;
        _mapper = mapper;
        const string tokenOptionsConfigurationSection = "TokenOptions";
        _tokenOptions =
            configuration.GetSection(tokenOptionsConfigurationSection).Get<TokenOptions>()
            ?? throw new NullReferenceException($"\"{tokenOptionsConfigurationSection}\" section cannot found in configuration");
    }


    public async Task<BaseRefreshToken> AddRefreshToken(BaseRefreshToken baseRefreshToken)
    {
        BaseRefreshToken addedBaseRefreshToken = await _refreshTokenRepository.AddAsync(baseRefreshToken); 

        return addedBaseRefreshToken;
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

    public Task<BaseRefreshToken> CreateRefreshToken(User user, string ipAdress)
    {
        BaseRefreshToken coreRefreshToken = _tokenHelper.CreateRefreshToken(user,ipAdress);

        BaseRefreshToken refreshToken = _mapper.Map<BaseRefreshToken>(coreRefreshToken);

        return Task.FromResult(refreshToken);
    }

    public async Task DeleteOldRefreshToken(Guid id,string ipAdress)
    {
        List<BaseRefreshToken> refreshTokens = await _refreshTokenRepository.GetOldRefreshTokensAsync(userId:id,refreshTokenTTL:_tokenOptions.RefreshTokenTTL,ipAdress);
        await _refreshTokenRepository.DeleteRangeAsync(refreshTokens);
    }

    public async Task<BaseRefreshToken?> GetRefreshTokenByToken(string refreshToken)
    {
        BaseRefreshToken? token = await _refreshTokenRepository.GetAsync(p=>p.Token==refreshToken);  
        return token;
    }

    public async Task<User?> GetUserAsync(Expression<Func<User, bool>> predicate, Func<IQueryable<User>, IIncludableQueryable<User, object>>? include = null, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        return await _userRepository.GetAsync
            (
            predicate:predicate,
            include:include,
            withDeleted:withDeleted,
            enableTracking:enableTracking,
            cancellationToken:cancellationToken
            );
    }

    public async Task RevokeDescendantRefreshTokens(BaseRefreshToken refreshToken, string ipAddress, string reason)
    {
        BaseRefreshToken? childToken = await _refreshTokenRepository.GetAsync(predicate: r =>
            r.Token == refreshToken.ReplacedByToken
        );

        if (childToken?.Revoked != null && childToken.Expires <= DateTime.UtcNow)
            await RevokeRefreshToken(childToken, ipAddress, reason);
        else
            await RevokeDescendantRefreshTokens(refreshToken: childToken!, ipAddress, reason);
    }

    public async Task RevokeRefreshToken(BaseRefreshToken token, string ipAddress, string? reason = null, string? replacedByToken = null)
    {
        token.Revoked = DateTime.UtcNow;
        token.RevokedByIp = ipAddress;
        token.ReasonRevoked = reason;
        token.ReplacedByToken = replacedByToken;
        await _refreshTokenRepository.UpdateAsync(token);
    }

    public async Task<BaseRefreshToken> RotateRefreshToken(User user, BaseRefreshToken refreshToken, string ipAddress)
    {
        BaseRefreshToken baseRefreshToken = _tokenHelper.CreateRefreshToken(user,ipAddress);
        BaseRefreshToken newRefreshToken = _mapper.Map<BaseRefreshToken>(baseRefreshToken);
        await RevokeRefreshToken(refreshToken,ipAddress,reason: "Replaced by new token", newRefreshToken.Token);

        return newRefreshToken;
    }

}
