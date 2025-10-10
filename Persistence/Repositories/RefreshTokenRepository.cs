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

public class RefreshTokenRepository : EfRepositoryBase<RefreshToken, Guid, BaseDbContext>,IRefreshTokenRepository
{
    public RefreshTokenRepository(BaseDbContext context) : base(context)
    {
    }
}
