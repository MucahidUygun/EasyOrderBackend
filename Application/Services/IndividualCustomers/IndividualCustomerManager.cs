using Application.Features.IndividualCustomers.Dtos.Requests;
using AutoMapper;
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

namespace Application.Services.IndividualCustomers;

public class IndividualCustomerManager : IIndividualCustomerService
{
    private readonly IIndividualCustomerRespository _repository;
    private readonly IMapper _mapper;

    public IndividualCustomerManager(IIndividualCustomerRespository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IndividualCustomer> AddAsync(IndividualCustomer individualCustomer, CancellationToken cancellationToken = default)
    {
        individualCustomer.Id = Guid.NewGuid();
        if (await _repository.AnyAsync(p => p.Id == individualCustomer.Id))
            await AddAsync(individualCustomer, cancellationToken);
        individualCustomer.IsActive = true;

        IndividualCustomer addedIndvidualCustomer = await _repository.AddAsync(individualCustomer, cancellationToken);

        return addedIndvidualCustomer;
    }
    public async Task<IndividualCustomer> UpdateAsync(UpdateIndividualCustomerRequest individualCustomer, CancellationToken cancellationToken = default)
    {
        IndividualCustomer? updatedIndividualCustomer = await _repository.GetAsync(p => p.Id == individualCustomer.Id, cancellationToken: cancellationToken);

        updatedIndividualCustomer = _mapper.Map(individualCustomer, updatedIndividualCustomer);

        await _repository.UpdateAsync(updatedIndividualCustomer!);

        return updatedIndividualCustomer;
    }

    public async Task<IndividualCustomer> DeleteAsync(DeleteIndividualCustomerRequest individualCustomer, bool permanent = false, CancellationToken cancellationToken = default)    
    {
        IndividualCustomer? deletedIndividualCustomer = await _repository.GetAsync(p => p.Id == individualCustomer.Id, cancellationToken: cancellationToken);
        deletedIndividualCustomer.IsActive = false;

        await _repository.DeleteAsync(deletedIndividualCustomer, permanent, cancellationToken);

        return deletedIndividualCustomer;
    }

    public async Task<IPaginate<IndividualCustomer>?> GetAllAsync(Expression<Func<IndividualCustomer, bool>>? predicate = null, Func<IQueryable<IndividualCustomer>, IOrderedQueryable<IndividualCustomer>>? orderBy = null, Func<IQueryable<IndividualCustomer>, IIncludableQueryable<IndividualCustomer, object>>? include = null, int index = 0, int size = 10, bool withDeleted = true, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        Paginate<IndividualCustomer> individualCustomers = await _repository.GetListAsync
            (
            predicate,
            orderBy,
            include,
            index,
            size,
            withDeleted,
            enableTracking,
            cancellationToken
            );

        return individualCustomers;
    }

    public async Task<IndividualCustomer> GetAsync(Expression<Func<IndividualCustomer, bool>> predicate, Func<IQueryable<IndividualCustomer>, IIncludableQueryable<IndividualCustomer, object>>? include = null, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        IndividualCustomer? individualCustomer = await _repository.GetAsync(
            predicate,
            include,
            withDeleted,
            enableTracking,
            cancellationToken
            );
        return individualCustomer;
    }

    public Task<bool> AnyAsync(Expression<Func<IndividualCustomer, bool>>? predicate = null, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        return _repository.AnyAsync
            (
            predicate: predicate,
            withDeleted: withDeleted,
            enableTracking: enableTracking,
            cancellationToken
            );
    }
}
