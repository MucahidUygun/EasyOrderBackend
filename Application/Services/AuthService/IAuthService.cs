using Application.Features.Auth.Dtos.Responses;
using Core.Entities;
using Core.Security.JWT;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.AuthService;

public interface IAuthService
{
    public Task<AccessToken> CreateAccessToken(User user);
    public Task<BaseRefreshToken> CreateRefreshToken(User user,string ipAdress);
    public Task<BaseRefreshToken?> GetRefreshTokenByToken(string refreshToken);
    public Task<BaseRefreshToken> AddRefreshToken(BaseRefreshToken refreshToken);
    public Task DeleteOldRefreshToken(BaseUser user,string ipAdress);
    public Task RevokeDescendantRefreshTokens(BaseRefreshToken refreshToken, string ipAddress, string reason);

    public Task RevokeRefreshToken(BaseRefreshToken token, string ipAddress, string? reason = null, string? replacedByToken = null);

    public Task<BaseRefreshToken> RotateRefreshToken(User user, BaseRefreshToken refreshToken, string ipAddress);

    public Task<User?> GetUserAsync(
        Expression<Func<User, bool>> predicate,
        Func<IQueryable<User>, IIncludableQueryable<User, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default);
    public Task<User> UpdateUserAsync(User user,CancellationToken cancellationToken = default);
    public Task<ExitedResponse> LogOut();
    public Task<EmailAuthenticator?> VeriyfEmailAsync(Guid Id,string activationKey);
    public Task VerifiedEmailAsync(EmailAuthenticator authenticator);
    public Task<EmailAuthenticator> CreateEmailVerifyAsync(User user);
    public Task<EmailAuthenticator?> UpdateEmailAuthenticatorAsync(EmailAuthenticator emailAuthenticator); 
    public Task<EmailAuthenticator?> GetEmailAuthenticatorAsync(
        Expression<Func<EmailAuthenticator, bool>> predicate,
        Func<IQueryable<EmailAuthenticator>, IIncludableQueryable<EmailAuthenticator, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default);
}
