using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Context;
using Persistence.Repositories;
using Persistence.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BaseDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("EasyOrder"))
        );

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IIndividualCustomerRespository, IndividualCustomerRepostitory>();
        services.AddScoped<ICorporateCustomerRepository, CorporateCustomerRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IUserOperationClaimRepository,UserOperationClaimRepository>();
        services.AddScoped<IOperationClaimRepository, OperationClaimRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}
