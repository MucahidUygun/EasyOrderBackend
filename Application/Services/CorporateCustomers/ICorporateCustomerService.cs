using Application.Features.CorporateCustomers.Dtos.Requests;
using Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.CorporateCustomers;

public interface ICorporateCustomerService
{
    public Task<CorporateCustomer> AddAsync(CorporateCustomer corporateCustomer , CancellationToken cancellationToken = default);
    public Task<CorporateCustomer> UpdateAsync(UpdateCorporateCustomerRequest corporateCustomer,CancellationToken cancellationToken = default);
    public Task<CorporateCustomer> DeleteAsync(DeleteCorporateCustomerRequest corporateCustomer,CancellationToken cancellationToken = default);
    public Task<CorporateCustomer> GetAsync(
        Expression<Func<CorporateCustomer, bool>> predicate,
        Func<IQueryable<CorporateCustomer>, IIncludableQueryable<CorporateCustomer, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
        );
    public Task<IPaginate<CorporateCustomer>> GetAllAsync(
        Expression<Func<CorporateCustomer, bool>>? predicate = null,
        Func<IQueryable<CorporateCustomer>, IOrderedQueryable<CorporateCustomer>>? orderBy = null,
        Func<IQueryable<CorporateCustomer>, IIncludableQueryable<CorporateCustomer, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
        );
    public Task<bool> AnyAsync(
        Expression<Func<CorporateCustomer, bool>>? predicate = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default);

}
