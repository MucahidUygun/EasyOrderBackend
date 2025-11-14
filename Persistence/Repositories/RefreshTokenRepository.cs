using Core.Application.Contracts.Security.Interfaces;
using Core.Entities;
using Core.Persistence.Repositories;
using Core.Security.Enums;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories;

public class RefreshTokenRepository :  EfRepositoryBase<BaseRefreshToken, Guid, BaseDbContext>, IRefreshTokenRepository
{
    public RefreshTokenRepository(BaseDbContext context) : base(context)
    {
    }
    public async Task<List<BaseRefreshToken>> GetOldRefreshTokensAsync(BaseUser user, string ipAdress)
    {
        List<BaseRefreshToken> tokens = await Query()
            .AsNoTracking()
            .Where(r =>
                r.UserId == user.Id
                && r.RevokedDate == null
                && r.ExpiresDate >= DateTime.UtcNow
                && r.CreatedByIp == ipAdress
            )
            .ToListAsync();

        return tokens;
    }
}
