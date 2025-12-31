using Core.Application.Contracts.Security.Interfaces;
using Core.Application.Contracts.Security.Services;
using Core.Constants;
using Core.CrossCuttingConcerns.Expeptions.Types;
using Core.Entities;
using Core.Security.Enums;
using Core.Security.JWT;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Pipelines.Authorization;

public class RefreshBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse> 
    where TRequest : IRequest<TResponse>, ISecuredRequest
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly HttpContext _context;
    private readonly IHttpService _httpService;
    private readonly IJwtService _jwtService;
    public RefreshBehavior(IHttpContextAccessor httpContextAccessor, IRefreshTokenService refreshTokenService, IHttpService httpService,IJwtService jwtService)
    {
        _jwtService = jwtService;
        _httpContextAccessor = httpContextAccessor;
        _refreshTokenService = refreshTokenService;
        _context = _httpContextAccessor.HttpContext;
        _httpService = httpService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var shouldRun = _context.Items[CoreMessages.FlagForRefreshBehavior];
        //RefreshBheavior' a girmesi için flag(işaret) var mı?
        if (shouldRun is not bool triggred|| !triggred )
            return await next();

        string? oldAccessToken = _httpService.GetAccessTokenFromHeaders();
        if (oldAccessToken is null)
            throw new AuthorizationException(CoreMessages.NotAuthorized);

        string? refreshTokenFromCookie = _httpService.GetRefreshTokenFromCookie();
        if (refreshTokenFromCookie is null)
            throw new AuthorizationException(CoreMessages.NotAuthorized);

        string? createdByIp = _httpService.GetByIpAdressFromHeaders(); 
        if (createdByIp is null) 
            throw new AuthorizationException(CoreMessages.NotAuthorized);

        //Guid id = Guid.Parse(input: _jwtService.GetIdFromOldAccesToken(oldAccessToken));

        //BaseUser user = new()
        //{
        //    Id = id,
        //    Email = _jwtService.GetEmailFromOldAccesToken(oldAccessToken),

        //};
        //List<BaseClaim> baseClaims = _jwtService.GetClaimsByKey(oldAccessToken,CoreMessages.CliamRole);

        //RefreshTokenValidType validType = await _refreshTokenService.GetRefreshTokenValidType(refreshTokenFromCookie, createdByIp!, user);

        //switch (validType)
        //{
        //    case RefreshTokenValidType.Active:
        //        RefreshToken createdRefreshToken = await _refreshTokenService.RotateRefreshToken(user, refreshTokenFromCookie, createdByIp);
        //        AccessToken accessToken = _refreshTokenService.CreateAccessToken(user, baseClaims);
        //        _httpService.SetAccessTokenAndRefreshTokenFromRequest(user, baseClaims, accessToken, createdRefreshToken);
        //        break;
        //    case RefreshTokenValidType.Expired:
        //        RefreshToken? revokeRefreshToken = await _refreshTokenService.GetRefreshTokenByToken(refreshTokenFromCookie, createdByIp);
        //        await _refreshTokenService.RevokeRefreshToken(revokeRefreshToken!, createdByIp, reason: CoreMessages.TokenExpired);
        //        throw new AuthorizationException(CoreMessages.LogIn);
        //    case RefreshTokenValidType.Deleted:
        //        RefreshToken? revokeAllRefreshToken = await _refreshTokenService.GetRefreshTokenByToken(refreshTokenFromCookie, createdByIp);
        //        await _refreshTokenService.RevokeDescendantRefreshTokens(revokeAllRefreshToken!, createdByIp,CoreMessages.TokenReuseDetected);
        //        break;
        //    case RefreshTokenValidType.NotFound:
        //        throw new AuthorizationException(CoreMessages.NotAuthorized);
        //}
        TResponse response = await next();
        return response;
    }
}
