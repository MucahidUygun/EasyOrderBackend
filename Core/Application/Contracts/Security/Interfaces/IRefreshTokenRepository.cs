using Core.Entities;
using Core.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Contracts.Security.Interfaces;

public interface IRefreshTokenRepository : IAsyncRepository<BaseRefreshToken, Guid>
{
    Task<List<BaseRefreshToken>> GetOldRefreshTokensAsync(Guid userId, int refreshTokenTTL, string ipAdress);
    Task<bool> IsValidRefreshToken(BaseRefreshToken refreshToken,CancellationToken cancellationToken=default);
}
