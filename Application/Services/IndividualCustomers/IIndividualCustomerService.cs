using Application.Features.Customers.Dtos.Requests;
using Application.Features.IndividualCustomers.Dtos.Requests;
using Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.IndividualCustomers;

public interface IIndividualCustomerService
{
    public Task<IndividualCustomer> AddAsync(IndividualCustomer individualCustomer, CancellationToken cancellationToken = default);
    public Task<IndividualCustomer> UpdateAsync(UpdateIndividualCustomerRequest individualCustomer, CancellationToken cancellationToken = default);
    public Task<IndividualCustomer> DeleteAsync(DeleteIndividualCustomerRequest individualCustomer, bool permanent = false, CancellationToken cancellationToken = default);
    public Task<IndividualCustomer> GetAsync
        (
        Expression<Func<IndividualCustomer, bool>> predicate,
        Func<IQueryable<IndividualCustomer>, IIncludableQueryable<IndividualCustomer, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
        );
    public Task<IPaginate<IndividualCustomer>?> GetAllAsync
        (
        Expression<Func<IndividualCustomer, bool>>? predicate = null,
        Func<IQueryable<IndividualCustomer>, IOrderedQueryable<IndividualCustomer>>? orderBy = null,
        Func<IQueryable<IndividualCustomer>, IIncludableQueryable<IndividualCustomer, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
        );

    public Task<bool> AnyAsync(
        Expression<Func<IndividualCustomer, bool>>? predicate = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default);
}
