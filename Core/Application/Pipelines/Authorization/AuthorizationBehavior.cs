using Core.Application.Contracts.Security.Interfaces;
using Core.CrossCuttingConcerns.Expeptions.Types;
using Core.Entities;
using Core.Security.Enums;
using Core.Security.Extensions;
using Core.Security.JWT;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Pipelines.Authorization;

public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ISecuredRequest
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly HttpContext _httpContext;
    public AuthorizationBehavior(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _httpContext = _httpContextAccessor.HttpContext;
    }

    /*
     * 15/11/2025 görev
    RefreshBehavior oluştur. ----
    RefreshBehavior' u ServiceRegistiration' a ekle -----
    AuthorizationBehavior' da accessTokenın boş olduğu kısımda flag koy ki RefreshBehaviora gitsin ----
    Jwt için incantce oluşturma Dipendicy injection yap
    cookiden set ve get işlemlerini behaviordan alarak daha temiz kod yap
     */

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        //Kullanıcı authenticated olduğunu kontrol ediyor
        if (!_httpContext.User.Claims.Any())
        {   //Kullanıcı authenticated olmadığı için refreshBehavior devreye girmesi için flag(işaret) bırakılıyor
            _httpContext.Items["TriggerRefreshBehavior"] = true;
            return await next();
        }
        if (request.Claims.Any())
        {   //Tokendan kullanıcı role bilgileri alınıyor ICollection burada ki kritik yerlerden!
            ICollection<string>? userRoleClaims = _httpContext.User.ClaimRoles() ?? [];
            //Burada tokendan alınınan role bilgileri ile request(classının veya yetki gereken yer) ile roller eşleşme durumuna bakılıyor.
            bool isNotMatchedAUserRoleClaimWithRequestRoles = userRoleClaims
                .FirstOrDefault(userRoleClaim =>
                    userRoleClaim == "Admin" || request.Claims.Contains(userRoleClaim)
                )
                == null;
            if (isNotMatchedAUserRoleClaimWithRequestRoles)
                throw new AuthorizationException("You are not authorized.");


        }
        TResponse response = await next();
        return response;
    }
}
