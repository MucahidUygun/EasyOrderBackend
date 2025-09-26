using Application.Features.Customers.Rules;
using Application.Services.Customers;
using Application.Services.IndividualCustomers;
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
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });
        services.AddScoped<ICustomerService, CustomerManager>();
        services.AddScoped<CustomerBusinessRules>();
        services.AddScoped<IIndividualCustomerService, IndividualCustomerManager>();
        return services;
    }
}
