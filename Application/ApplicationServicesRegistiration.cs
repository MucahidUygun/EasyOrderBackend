using Application.Features.Customers.Rules;
using Application.Services.AuthService;
using Application.Services.CorporateCustomers;
using Application.Services.Customers;
using Application.Services.Employees;
using Application.Services.IndividualCustomers;
using Application.Services.OperationClaims;
using Core.Application.Pipelines.Authorization;
using Core.Application.Pipelines.Logging;
using Core.Application.Rules;
using Core.CrossCuttingConcerns.Logging.Abstraction;
using Core.CrossCuttingConcerns.Logging.Configurations;
using Core.CrossCuttingConcerns.Logging.Serilog;
using Core.Entities;
using Core.Mailing;
using Core.Mailing.MailKitImplementations;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Application;

public static class ApplicationServicesRegistiration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        MongoDbLogConfiguration mongoDbLogConfiguration, FileLogConfiguration fileLogConfiguration,
        MailSettings mailSettings)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            //Claim kontrolünün sağlanması için altta ki satır eklendi!
            configuration.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
            configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));
            configuration.AddOpenBehavior(typeof(RefreshBehavior<,>));

        });
        services.AddSingleton<IMailService, MailKitMailService>(_ => new MailKitMailService(mailSettings));
        services.AddSingleton<ILogger, SerilogMongoDbWithFileLogger>(_ => new SerilogMongoDbWithFileLogger(mongoDbLogConfiguration,fileLogConfiguration));
        services.AddSubClassesOfType(Assembly.GetExecutingAssembly(), typeof(BaseBusinessRules));
        services.AddScoped<ICustomerService, CustomerManager>();
        services.AddScoped<IIndividualCustomerService, IndividualCustomerManager>();
        services.AddScoped<ICorporateCustomerService, CorporateCustomerManager>();
        services.AddScoped<IOperationClaimService, OperationClaimManager>();
        services.AddScoped<IAuthService, AuthManager>();
        services.AddScoped<IEmployeeService, EmplooyeManager>();
        //services.AddScoped<BaseRefreshToken, RefreshToken>();
        return services;
    }

    public static IServiceCollection AddSubClassesOfType(
        this IServiceCollection services,
        Assembly assembly,
        Type type,
        Func<IServiceCollection, Type, IServiceCollection>? addWithLifeCycle = null
    )
    {
        var types = assembly.GetTypes().Where(t => t.IsSubclassOf(type) && type != t).ToList();
        foreach (Type? item in types)
            if (addWithLifeCycle == null)
                services.AddScoped(item);
            else
                addWithLifeCycle(services, type);
        return services;
    }
}
