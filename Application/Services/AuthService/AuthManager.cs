using Application.Features.Auth.Dtos.Responses;
using AutoMapper;
using Core.Application.Contracts.Security.Interfaces;
using Core.CrossCuttingConcerns.Expeptions.Types;
using Core.Entities;
using Core.Security.Hashing;
using Core.Security.JWT;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Configuration;
using Persistence.Services;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.AuthService;

public class AuthManager : IAuthService
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenHelper _tokenHelper;
    private readonly IUserOperationClaimRepository _userOperationClaimRepository;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly IEmailAuthenticatorRepository _emailAuthenticatorRepository;
    private readonly IHttpService _httpService;

    public AuthManager(
    IRefreshTokenRepository refreshTokenRepository,
    ITokenHelper tokenHelper,
    IMapper mapper,
    IUserOperationClaimRepository userOperationClaimRepository,
    IUserRepository userRepository,
    IEmailAuthenticatorRepository emailAuthenticatorRepository,
    IHttpService httpService)
    {
        _userRepository = userRepository;
        _userOperationClaimRepository = userOperationClaimRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _tokenHelper = tokenHelper;
        _mapper = mapper;
        _emailAuthenticatorRepository = emailAuthenticatorRepository;
        _httpService = httpService;
    }


    public async Task<BaseRefreshToken> AddRefreshToken(RefreshToken baseRefreshToken)
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
        if (_httpService.GetDeviceIdAdressFromHeaders() is null
            && _httpService.GetUserAgentFromHeaders() is null
            && _httpService.GetDevicePlatformFromHeaders() is null
            )
            throw new AuthorizationException("You are not auth");

        BaseRefreshToken refreshToken = _tokenHelper.CreateRefreshToken(
            user:user,
            ipAdress:ipAdress,
            deviceId:_httpService.GetDeviceIdAdressFromHeaders()!,
            userAgent:_httpService.GetUserAgentFromHeaders()!,
            devicePlatform:_httpService.GetDevicePlatformFromHeaders()!,
            deviceName:_httpService.GetDeviceNameFromHeaders());

        return Task.FromResult(refreshToken);
    }

    public async Task DeleteOldRefreshToken(BaseUser user,string newToken,string reason)
    {
        IEnumerable<BaseRefreshToken> refreshTokens = await _refreshTokenRepository.GetOldRefreshTokensAsync(
            user:user,
            ipAdress: _httpService.GetByIpAdressFromHeaders()!,
            deviceId: _httpService.GetDeviceIdAdressFromHeaders()!,
            deviceName: _httpService.GetDeviceNameFromHeaders()!,
            userAgent: _httpService.GetUserAgentFromHeaders()!,
            platform: _httpService.GetDevicePlatformFromHeaders()!
            );
        ICollection<BaseRefreshToken> tokens =refreshTokens.ToList();
        foreach (BaseRefreshToken token in tokens)
        {
            token.RevokedDate = DateTime.UtcNow;
            token.RevokedByIp = _httpService.GetByIpAdressFromHeaders();
            token.ReasonRevoked = reason;
            token.ReplacedByToken = newToken;
            token.IsActive = false;
            token.DeletedDate = DateTime.UtcNow;
        }
        await _refreshTokenRepository.DeleteRangeAsync(tokens);
    }

    public async Task<BaseRefreshToken?> GetRefreshTokenByToken(string refreshToken)
    {
        return await _refreshTokenRepository.GetAsync(p=>p.Token==refreshToken);
    }

    public async Task<BaseRefreshToken?> GetRefreshTokenAsync(Expression<Func<BaseRefreshToken, bool>> predicate, Func<IQueryable<BaseRefreshToken>, IIncludableQueryable<BaseRefreshToken, object>>? include = null, bool withDeleted = false, bool enableTracking = false, CancellationToken cancellationToken = default)
    {
        return  await _refreshTokenRepository.GetAsync(predicate, include, withDeleted, enableTracking, cancellationToken);
    }

    public async Task<User?> GetUserAsync(Expression<Func<User, bool>> predicate, Func<IQueryable<User>, IIncludableQueryable<User, object>>? include = null, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        return await _userRepository.GetAsync
            (
            predicate: predicate,
            include: include,
            withDeleted: withDeleted,
            enableTracking: enableTracking,
            cancellationToken: cancellationToken
            );
    }

    public async Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken)
    {
        return await _userRepository.UpdateAsync(user,cancellationToken);
    }

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

    public async Task<BaseRefreshToken> RotateRefreshToken(User user, BaseRefreshToken refreshToken, string ipAdress)
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

        await RevokeRefreshToken(refreshToken,ipAdress,reason: "Replaced by new token", baseRefreshToken.Token);

        return baseRefreshToken;
    }

    public async Task<EmailAuthenticator?> VeriyfEmailAsync(Guid Id, string activationKey)
    {
       return await _emailAuthenticatorRepository.GetAsync(p=>p.UserId==Id && p.ActivationKey==CustomEncoders.UrlDecode(activationKey));
    }
    public async Task VerifiedEmailAsync(EmailAuthenticator authenticator)
    {
        EmailAuthenticator? emailAuthenticator = await _emailAuthenticatorRepository.GetAsync(p=>p.Id==authenticator.Id);
        emailAuthenticator!.IsActive = true;
        emailAuthenticator.ActivationKey = null;
        User? user = await _userRepository.GetAsync(p=>p.Id==emailAuthenticator.UserId);
        if (user is null)
            throw new BusinessException("User not exists");
        user.IsActive = true;

        await _emailAuthenticatorRepository.UpdateAsync(emailAuthenticator);
        await _userRepository.UpdateAsync(user);
    }

    public async Task<EmailAuthenticator> CreateEmailVerifyAsync(User user)
    {
        EmailAuthenticator emailAuthenticator = new()
        {
            UserId = user.Id,
            ActivationKey = HashingHelper.CreateActivationKey(HashingHelper.GenerateRandomKey(6)),
            VerifyEmailTokenExpiry = DateTime.Now.AddMinutes(10),
        };
        await _emailAuthenticatorRepository.AddAsync(emailAuthenticator,IsActive:false);
        return emailAuthenticator;
    }

    public async Task<EmailAuthenticator?> UpdateEmailAuthenticatorAsync(EmailAuthenticator emailAuthenticator)
    {
        return await _emailAuthenticatorRepository.UpdateAsync(emailAuthenticator);
    }

    public async Task<EmailAuthenticator?> GetEmailAuthenticatorAsync(
        Expression<Func<EmailAuthenticator, bool>> predicate, 
        Func<IQueryable<EmailAuthenticator>, IIncludableQueryable<EmailAuthenticator, object>>? include = null, 
        bool withDeleted = false, 
        bool enableTracking = true, 
        CancellationToken cancellationToken = default)
    {
        return await _emailAuthenticatorRepository.GetAsync(predicate, include, withDeleted, enableTracking,cancellationToken);
    }

   

}
