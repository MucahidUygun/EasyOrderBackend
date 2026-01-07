using Core.Entities;
using Core.Persistence.Paging;
using Core.Persistence.Repositories;
using Core.Security.Enums;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Contracts.Security.Interfaces;

public interface IRefreshTokenRepository
{
    Task<BaseRefreshToken?> GetAsync(
        Expression<Func<BaseRefreshToken, bool>> predicate,
        Func<IQueryable<BaseRefreshToken>, IIncludableQueryable<BaseRefreshToken, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default);

    // Get List
    Task<Paginate<BaseRefreshToken>> GetListAsync(
        Expression<Func<BaseRefreshToken, bool>>? predicate = null,
        Func<IQueryable<BaseRefreshToken>, IOrderedQueryable<BaseRefreshToken>>? orderBy = null,
        Func<IQueryable<BaseRefreshToken>, IIncludableQueryable<BaseRefreshToken, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default);

    // Get List
    Task<List<BaseRefreshToken?>> GetListNotPaginateAsync(
        Expression<Func<BaseRefreshToken, bool>>? predicate = null,
        Func<IQueryable<BaseRefreshToken>, IOrderedQueryable<BaseRefreshToken>>? orderBy = null,
        Func<IQueryable<BaseRefreshToken>, IIncludableQueryable<BaseRefreshToken, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default);

    // Any
    Task<bool> AnyAsync(
        Expression<Func<BaseRefreshToken, bool>>? predicate = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default);

    // Add
    Task<BaseRefreshToken> AddAsync(BaseRefreshToken entity, bool IsActive = true, CancellationToken cancellationToken = default);

    // Update
    Task<BaseRefreshToken> UpdateAsync(BaseRefreshToken entity, CancellationToken cancellationToken = default);
    Task<ICollection<BaseRefreshToken>> UpdateRangeAsync(ICollection<BaseRefreshToken> entities, CancellationToken cancellationToken = default);

    // Delete
    Task<BaseRefreshToken> DeleteAsync(BaseRefreshToken entity, bool permanent = false, CancellationToken cancellationToken = default);
    Task<ICollection<BaseRefreshToken>> DeleteRangeAsync(ICollection<BaseRefreshToken> entities, bool permanent = false, CancellationToken cancellationToken = default);
    //En Sonda ki veriyi Getirir
    public Task<BaseRefreshToken?> GetLastAsync(
       Expression<Func<BaseRefreshToken, object>> orderBySelector,
       bool withDeleted = false,
       bool enableTracking = true,
       CancellationToken cancellationToken = default);
    Task<IEnumerable<BaseRefreshToken>> GetOldRefreshTokensAsync(BaseUser user,string ipAdress, string deviceId, string deviceName, string userAgent, string platform);
}
