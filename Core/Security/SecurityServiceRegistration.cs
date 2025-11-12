using Core.Application.Contracts.Security.Interfaces;
using Core.Application.Contracts.Security.Services;
using Core.Security.JWT;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Security;

public static class SecurityServiceRegistration
{
    public static IServiceCollection AddSecurityServices(this IServiceCollection services)
    {
        services.AddScoped<ITokenHelper, TokenHelper>();
        services.AddScoped<IRefreshTokenService, RefreshTokenManager>();
        return services;
    }
}