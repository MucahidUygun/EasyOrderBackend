using Core.Persistence.Repositories;
using Domain.Entities;
using Persistence.Context;
using Persistence.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories;

public class EmployeeRepository : EfRepositoryBase<Employee, Guid, BaseDbContext>, IEmpoyeeRepository
{
    public EmployeeRepository(BaseDbContext context) : base(context)
    {
    }
}
