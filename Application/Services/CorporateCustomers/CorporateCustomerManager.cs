using Application.Features.CorporateCustomers.Constants;
using Application.Features.CorporateCustomers.Dtos.Requests;
using AutoMapper;
using Core.CrossCuttingConcerns.Expeptions.Types;
using Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using Persistence.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.CorporateCustomers;

public class CorporateCustomerManager : ICorporateCustomerService
{
    private readonly ICorporateCustomerRepository _repository;
    private readonly IMapper _mapper;

    public CorporateCustomerManager(ICorporateCustomerRepository repostitory, IMapper mapper)
    {
        _repository = repostitory;
        _mapper = mapper;
    }

    public async Task<CorporateCustomer> AddAsync(CorporateCustomer corporateCustomer, CancellationToken cancellationToken = default)
    {
        corporateCustomer.Id = Guid.NewGuid();
        if(await _repository.AnyAsync(p=>p.Id== corporateCustomer.Id))
           await AddAsync(corporateCustomer, cancellationToken);

        corporateCustomer.IsActive = true;
        await _repository.AddAsync(corporateCustomer);

        return corporateCustomer;
    }

    public Task<bool> AnyAsync(Expression<Func<CorporateCustomer, bool>>? predicate = null, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        return _repository.AnyAsync(
            predicate: predicate,
            withDeleted: withDeleted,
            enableTracking: enableTracking,
            cancellationToken:cancellationToken
            );
    }

    public async Task<CorporateCustomer> DeleteAsync(DeleteCorporateCustomerRequest corporateCustomer, CancellationToken cancellationToken = default)
    {
        CorporateCustomer? deletedCustomer = await _repository.GetAsync(p=>p.Id == corporateCustomer.Id,cancellationToken:cancellationToken);

        deletedCustomer!.IsActive = false;
        await _repository.DeleteAsync(deletedCustomer);

        return deletedCustomer;
    }

    public async Task<IPaginate<CorporateCustomer>> GetAllAsync(Expression<Func<CorporateCustomer, bool>>? predicate = null, Func<IQueryable<CorporateCustomer>, IOrderedQueryable<CorporateCustomer>>? orderBy = null, Func<IQueryable<CorporateCustomer>, IIncludableQueryable<CorporateCustomer, object>>? include = null, int index = 0, int size = 10, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        IPaginate<CorporateCustomer> corporateCustomers = await _repository.GetListAsync(
            predicate:predicate,
            orderBy:orderBy,
            include:include,
            index:index,
            size:size,
            withDeleted:withDeleted,
            enableTracking:enableTracking,
            cancellationToken
            );


        return corporateCustomers;
    }

    public async Task<CorporateCustomer> GetAsync(Expression<Func<CorporateCustomer, bool>> predicate, Func<IQueryable<CorporateCustomer>, IIncludableQueryable<CorporateCustomer, object>>? include = null, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        CorporateCustomer? corporateCustomer = await _repository.GetAsync(
            predicate: predicate,
            include: include,
            withDeleted: withDeleted,
            enableTracking: enableTracking,
            cancellationToken
            );

        return corporateCustomer ?? throw new BusinessException(CorporateCustomerBusinessMessages.CorporateCustomerNotExists);
    }

    public async Task<CorporateCustomer> UpdateAsync(UpdateCorporateCustomerRequest corporateCustomer, CancellationToken cancellationToken = default)
    {
        CorporateCustomer? updatedCustomer = await _repository.GetAsync(predicate: p => p.Id == corporateCustomer.Id, cancellationToken: cancellationToken);

        updatedCustomer = _mapper.Map(corporateCustomer, updatedCustomer);

        await _repository.UpdateAsync(updatedCustomer!);

        return updatedCustomer ?? throw new BusinessException(CorporateCustomerBusinessMessages.CorporateCustomerNotExists);
    }
}
