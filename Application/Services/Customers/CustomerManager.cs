using Application.Features.Customers.Constants;
using Application.Features.Customers.Dtos.Requests;
using Application.Features.Customers.Dtos.Responses;
using AutoMapper;
using Core.CrossCuttingConcerns.Expeptions.Types;
using Core.Persistence.Paging;
using Core.Security.Hashing;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using Persistence.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Customers;

public class CustomerManager : ICustomerService
{

    private readonly ICustomerRepository _repository;
    private readonly IMapper _mapper;

    public CustomerManager(ICustomerRepository repository,IMapper mapper)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<Customer> AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        customer.Id = new Guid();
        if (await _repository.AnyAsync(p => p.Id == customer.Id))
            await AddAsync(customer, cancellationToken);
        customer.IsActive = true;
        Customer addedCustomer = await _repository.AddAsync(customer,cancellationToken: cancellationToken);
        return addedCustomer;
    }

    public async Task<Customer> UpdateAsync(UpdateCustomerRequest customer, CancellationToken cancellationToken = default)
    {
        Customer? updatedCustomer = await _repository.GetAsync(predicate: p => p.Id == customer.Id, cancellationToken: cancellationToken);

        updatedCustomer = _mapper.Map(customer,updatedCustomer);

        await _repository.UpdateAsync(updatedCustomer!);

        return updatedCustomer;
    }

    public async Task<Customer> DeleteAsync(DeleteCustomerRequest customer, bool permanent = false, CancellationToken cancellationToken = default)
    {
        Customer? deletedCustomer = await _repository.GetAsync(predicate: p=>p.Id==customer.Id,cancellationToken:cancellationToken);
        deletedCustomer.IsActive = false;

        await _repository.DeleteAsync(deletedCustomer,permanent,cancellationToken);

        return deletedCustomer;
    }

    public async Task<IPaginate<Customer>?> GetAllAsync(Expression<Func<Customer, bool>>? predicate = null, Func<IQueryable<Customer>, IOrderedQueryable<Customer>>? orderBy = null, Func<IQueryable<Customer>, IIncludableQueryable<Customer, object>>? include = null, int index = 0, int size = 10, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        Paginate<Customer> customers = await _repository.GetListAsync
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

        return customers;
    }

    public async Task<Customer> GetAsync(Expression<Func<Customer, bool>> predicate, Func<IQueryable<Customer>, IIncludableQueryable<Customer, object>>? include = null, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        Customer? customer = await _repository.GetAsync(
            predicate,
            include,
            withDeleted,
            enableTracking,
            cancellationToken
            );
        return customer ?? throw new BusinessException(CustomersBusinessMessages.CustomerNotExists);
    }

    public Task<bool> AnyAsync(Expression<Func<Customer, bool>>? predicate = null, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        return _repository.AnyAsync
            (
            predicate:predicate,
            withDeleted:withDeleted,
            enableTracking:enableTracking,
            cancellationToken
            );
    }
}
