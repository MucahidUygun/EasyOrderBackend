using Core.Application.Contracts.Security.Interfaces;
using Core.Entities;
using Core.Persistence.Repositories;
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

    public async Task<bool> IsValidRefreshToken(BaseRefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        var tokenEntity = await Query()
                .FirstOrDefaultAsync(x => x.Token == refreshToken.Token, cancellationToken);

        if (tokenEntity is null) return false;
        if (tokenEntity.Revoked <= DateTime.Now) return false;
        if (tokenEntity.Expires < DateTime.UtcNow) return false;

        return true;
    }
}
