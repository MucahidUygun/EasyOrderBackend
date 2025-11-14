using Core.Entities;
using Core.Persistence.Repositories;
using Core.Security.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Contracts.Security.Interfaces;

public interface IRefreshTokenRepository : IAsyncRepository<BaseRefreshToken, Guid>
{
    Task<List<BaseRefreshToken>> GetOldRefreshTokensAsync(BaseUser user,string ipAdress);
}
