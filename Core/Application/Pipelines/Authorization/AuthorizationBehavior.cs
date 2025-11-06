using Core.Application.Contracts.Security.Interfaces;
using Core.CrossCuttingConcerns.Expeptions.Types;
using Core.Entities;
using Core.Security.Extensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Pipelines.Authorization;

public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ISecuredRequest
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRefreshTokenRepository _refreshTokenValidator;
    public AuthorizationBehavior(IHttpContextAccessor httpContextAccessor,IRefreshTokenRepository refreshTokenValidator)
    {
        _refreshTokenValidator = refreshTokenValidator;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        //Kullanıcı authenticated olduğunu kontrol ediyor
        if (!_httpContextAccessor.HttpContext.User.Claims.Any())
            throw new AuthorizationException("You are not authenticated.");

        if (request.Claims.Any())
        {   //Tokendan kullanıcı role bilgileri alınıyor ICollection burada ki kritik yerlerden!
            ICollection<string>? userRoleClaims = _httpContextAccessor.HttpContext.User.ClaimRoles() ?? [];
            //Burada tokendan alınınan role bilgileri ile request(classının veya yetki gereken yer) ile roller eşleşme durumuna bakılıyor.
            bool isNotMatchedAUserRoleClaimWithRequestRoles = userRoleClaims
                .FirstOrDefault(userRoleClaim =>
                    userRoleClaim == "Admin" || request.Claims.Contains(userRoleClaim)
                )
                == null;
            if (isNotMatchedAUserRoleClaimWithRequestRoles)
                throw new AuthorizationException("You are not authorized.");

            string token = _httpContextAccessor.HttpContext.Request.Cookies.FirstOrDefault(p => p.Key.Equals("refreshToken")).Value;
            BaseRefreshToken? refreshToken = await _refreshTokenValidator.GetAsync(p => p.Token == token, cancellationToken: cancellationToken);

            if (string.IsNullOrWhiteSpace(refreshToken?.Token))
                throw new AuthorizationException("Refresh token is required for this request.");

            bool valid = await _refreshTokenValidator.IsValidRefreshToken(refreshToken);

            if (!valid)
                throw new AuthorizationException("Invalid or expired refresh token");
        }

        //if (request is IHasRefreshToken refreshable)
        //{
       


        //}

        TResponse response = await next();
        return response;
    }
}
