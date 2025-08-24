using Core.Persistence.Repositories;
using Domain.Entities;
using Persistence.Context;
using Persistence.Services;

namespace Persistence.Repositories;

public class CustomerRepository: EfRepositoryBase<Customer,Guid,BaseDbContext>,
    ICustomerRepository 
{
    public CustomerRepository(BaseDbContext context) : base(context)
    {
    }
}
    