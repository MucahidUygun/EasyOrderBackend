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
    public async Task<List<BaseRefreshToken>> GetOldRefreshTokensAsync(Guid userId, int refreshTokenTtl, string ipAdress)
    {
        List<BaseRefreshToken> tokens = await Query()
            .AsNoTracking()
            .Where(r =>
                r.UserId == userId
                && r.Revoked == null
                && r.Expires >= DateTime.UtcNow
                && r.CreatedByIp == ipAdress
            )
            .ToListAsync();

        return tokens;
    }

    public async Task<RefreshTokenValidType> IsValidRefreshToken(string refreshToken, CancellationToken cancellationToken = default)
    {
        var tokenEntity = await GetAsync(x=>x.Token ==refreshToken,withDeleted:true,cancellationToken:cancellationToken);
       
        if (tokenEntity is null) return RefreshTokenValidType.NotFound;
        if (tokenEntity.Revoked <= DateTime.Now) return RefreshTokenValidType.Expired;
        if (tokenEntity.Expires < DateTime.UtcNow) return RefreshTokenValidType.Expired;

        return RefreshTokenValidType.Active;
    }
}
