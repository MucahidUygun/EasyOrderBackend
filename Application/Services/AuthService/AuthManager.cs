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
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEmailAuthenticatorRepository _emailAuthenticatorRepository;

    public AuthManager(
    IRefreshTokenRepository refreshTokenRepository,
    ITokenHelper tokenHelper,
    IMapper mapper,
    IUserOperationClaimRepository userOperationClaimRepository,
    IUserRepository userRepository,
    IHttpContextAccessor httpContextAccessor
,
    IEmailAuthenticatorRepository emailAuthenticatorRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
        _userOperationClaimRepository = userOperationClaimRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _tokenHelper = tokenHelper;
        _mapper = mapper;
        _emailAuthenticatorRepository = emailAuthenticatorRepository;
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

    public async Task DeleteOldRefreshToken(BaseUser user,string ipAdress)
    {
        List<BaseRefreshToken> refreshTokens = await _refreshTokenRepository.GetOldRefreshTokensAsync(user,ipAdress);
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
            predicate: predicate,
            include: include,
            withDeleted: withDeleted,
            enableTracking: enableTracking,
            cancellationToken: cancellationToken
            );
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

    public async Task<BaseRefreshToken> RotateRefreshToken(User user, BaseRefreshToken refreshToken, string ipAddress)
    {
        BaseRefreshToken baseRefreshToken = _tokenHelper.CreateRefreshToken(user,ipAddress);
        BaseRefreshToken newRefreshToken = _mapper.Map<BaseRefreshToken>(baseRefreshToken);
        await RevokeRefreshToken(refreshToken,ipAddress,reason: "Replaced by new token", newRefreshToken.Token);

        return newRefreshToken;
    }

    public async Task<ExitedResponse> LogOut()
    {
        string? refreshToken = GetRefreshTokenFromCookie();
        if (refreshToken is null)
            return new () { Message = "Refresh token not exists",Status = false };
        string? ipAdress = GetByIpAdressFromHeaders();
        if (ipAdress is null)
            return new () { Message = "IpAdress not exists",Status = false };
        BaseRefreshToken? baseRefresh = await _refreshTokenRepository.GetAsync(p=>p.Token==refreshToken && p.CreatedByIp==ipAdress);
        if (baseRefresh is null)
            return new () {Message = "Refresh token not exists",Status = false};
        await RevokeDescendantRefreshTokens(baseRefresh,ipAdress,"LogOut");
        DeleteRefreshTokenFromCookie();
        return new () { Message = "Logout is success",Status = true };
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
    public string? GetByIpAdressFromHeaders()
    {
        return _httpContextAccessor.HttpContext!.Request.Headers.ContainsKey("X-Forwarded-For")
        ? _httpContextAccessor.HttpContext!.Request.Headers["X-Forwarded-For"].ToString()
        : _httpContextAccessor.HttpContext!.Connection.RemoteIpAddress?.MapToIPv4().ToString();
    }

    public string? GetRefreshTokenFromCookie()
    {
        return _httpContextAccessor.HttpContext!.Request.Cookies.FirstOrDefault(p => p.Key.Equals("refreshToken")).Value;
    }

    public void DeleteRefreshTokenFromCookie()
    {
       _httpContextAccessor.HttpContext!.Response.Cookies.Delete("refreshToken");
    }

}
