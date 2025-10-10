using Core.CrossCuttingConcerns.Expeptions.Types;
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

    public AuthorizationBehavior(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        List<string>? roleClaims = _httpContextAccessor.HttpContext.User.ClaimRoles();

        if (roleClaims == null)
            throw new AuthorizationException("Claim not found!");

        bool isNotMatchedARoleClaimWithRequestRoles =
            string.IsNullOrEmpty(roleClaims.FirstOrDefault(
                roleClaim => request.Claims.Any(role => role == roleClaim)
            ));

        if (isNotMatchedARoleClaimWithRequestRoles)
            throw new AuthorizationException("You are not authorization");

        return next();
    }
}
