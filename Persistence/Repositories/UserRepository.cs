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

public class UserRepository : EfRepositoryBase<User, Guid, BaseDbContext>, IUserRepository
{
    public UserRepository(BaseDbContext context) : base(context)
    {
    }
}
