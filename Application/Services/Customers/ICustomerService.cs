using Application.Features.Customers.Dtos.Requests;
using Application.Features.Customers.Dtos.Responses;
using Core.Persistence.Paging;
using Core.Persistence.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Customers;

public interface ICustomerService
{
    public Task<Customer> AddAsync(Customer customer, CancellationToken cancellationToken = default);
    public Task<Customer> UpdateAsync(UpdateCustomerRequest customer, CancellationToken cancellationToken = default);
    public Task<Customer> DeleteAsync(DeleteCustomerRequest customer ,bool permanent = false ,CancellationToken cancellationToken = default);
    public Task<Customer> GetAsync
        (
        Expression<Func<Customer, bool>> predicate,
        Func<IQueryable<Customer>, IIncludableQueryable<Customer, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
        );
    public Task<IPaginate<Customer>?> GetAllAsync
        (
        Expression<Func<Customer, bool>>? predicate = null,
        Func<IQueryable<Customer>, IOrderedQueryable<Customer>>? orderBy = null,
        Func<IQueryable<Customer>, IIncludableQueryable<Customer, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
        );
}
