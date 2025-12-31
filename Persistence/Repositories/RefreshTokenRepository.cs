using Core.Application.Contracts.Security.Interfaces;
using Core.Entities;
using Core.Persistence.Paging;
using Core.Persistence.Repositories;
using Core.Security.Enums;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Org.BouncyCastle.Tls;
using Persistence.Context;
using Persistence.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    protected readonly DbContext Context;
    public RefreshTokenRepository(BaseDbContext context) 
    {
        Context = context;
    }
    public IQueryable<RefreshToken> Query()
    {
        return Context.Set<RefreshToken>();
    }

    public async Task<BaseRefreshToken?> GetAsync(
        Expression<Func<BaseRefreshToken, bool>> predicate,
        Func<IQueryable<BaseRefreshToken>, IIncludableQueryable<BaseRefreshToken, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<BaseRefreshToken> queryable = Context.Set<RefreshToken>();

        if (!enableTracking)
            queryable = queryable.AsNoTracking();

        if (include != null)
            queryable = include(queryable);

        if (!withDeleted)
            queryable = queryable.Where(x => x.DeletedDate == null);

        return await queryable.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<Paginate<BaseRefreshToken>> GetListAsync(
        Expression<Func<BaseRefreshToken, bool>>? predicate = null,
        Func<IQueryable<BaseRefreshToken>, IOrderedQueryable<BaseRefreshToken>>? orderBy = null,
        Func<IQueryable<BaseRefreshToken>, IIncludableQueryable<BaseRefreshToken, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<BaseRefreshToken> queryable = Context.Set<RefreshToken>();

        if (!enableTracking)
            queryable = queryable.AsNoTracking();

        if (include != null)
            queryable = include(queryable);

        if (!withDeleted)
            queryable = queryable.Where(x => x.DeletedDate == null);

        if (predicate != null)
            queryable = queryable.Where(predicate);

        if (orderBy != null)
            queryable = orderBy(queryable);

        var totalItems = await queryable.CountAsync(cancellationToken);
        var items = await queryable.Skip(index * size).Take(size).ToListAsync(cancellationToken);

        return new Paginate<BaseRefreshToken>
        {
            Items = items,
            PageNumber = index + 1,
            PageSize = size,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)size)
        };
    }

    public async Task<List<BaseRefreshToken?>> GetListNotPaginateAsync(Expression<Func<BaseRefreshToken, bool>>? predicate = null, Func<IQueryable<BaseRefreshToken>, IOrderedQueryable<BaseRefreshToken>>? orderBy = null, Func<IQueryable<BaseRefreshToken>, IIncludableQueryable<BaseRefreshToken, object>>? include = null, int index = 0, int size = 10, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        IQueryable<BaseRefreshToken> queryable = Context.Set<RefreshToken>();

        if (!enableTracking)
            queryable = queryable.AsNoTracking();

        if (include != null)
            queryable = include(queryable);

        if (!withDeleted)
            queryable = queryable.Where(x => x.DeletedDate == null);

        if (predicate != null)
            queryable = queryable.Where(predicate);

        if (orderBy != null)
            queryable = orderBy(queryable);

        var totalItems = await queryable.CountAsync(cancellationToken);
        var items = await queryable.Skip(index * size).Take(size).ToListAsync(cancellationToken);

        return items!;
    }

    public async Task<bool> AnyAsync(
        Expression<Func<BaseRefreshToken, bool>>? predicate = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<BaseRefreshToken> queryable = Context.Set<RefreshToken>();

        if (!enableTracking)
            queryable = queryable.AsNoTracking();

        if (!withDeleted)
            queryable = queryable.Where(x => x.DeletedDate == null);

        if (predicate != null)
            queryable = queryable.Where(predicate);

        return await queryable.AnyAsync(cancellationToken);
    }

    public async Task<BaseRefreshToken> AddAsync(BaseRefreshToken entity, bool IsActive = true, CancellationToken cancellationToken = default)
    {
        entity.IsActive = IsActive;
        entity.CreatedDate = DateTime.UtcNow;
        await Context.Set<RefreshToken>().AddAsync((RefreshToken)entity, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
        return entity;
    }

   

    public async Task<BaseRefreshToken> UpdateAsync(BaseRefreshToken entity, CancellationToken cancellationToken = default)
    {
        entity.UpdatedDate = DateTime.UtcNow;
        Context.Set<RefreshToken>().Update((RefreshToken)entity);
        await Context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<ICollection<BaseRefreshToken>> UpdateRangeAsync(ICollection<BaseRefreshToken> entities, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
            entity.UpdatedDate = DateTime.UtcNow;

        Context.Set<RefreshToken>().UpdateRange((RefreshToken)entities);
        await Context.SaveChangesAsync(cancellationToken);
        return entities;
    }

    public async Task<BaseRefreshToken> DeleteAsync(BaseRefreshToken entity, bool permanent = false, CancellationToken cancellationToken = default)
    {
        if (!permanent)
        {
            entity.DeletedDate = DateTime.UtcNow;
            entity.IsActive = false;
            Context.Set<RefreshToken>().Update((RefreshToken)entity);
        }
        else
        {
            Context.Set<RefreshToken>().Remove((RefreshToken)entity);
        }

        await Context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<BaseRefreshToken?> GetLastAsync(
       Expression<Func<BaseRefreshToken, object>> orderBySelector,
       bool withDeleted = false,
       bool enableTracking = true,
       CancellationToken cancellationToken = default)
    {
        IQueryable<BaseRefreshToken> queryable = Context.Set<RefreshToken>();

        if (!enableTracking)
            queryable = queryable.AsNoTracking();

        if (!withDeleted)
            queryable = queryable.Where(x => x.DeletedDate == null);

        return await queryable
            .OrderByDescending(orderBySelector)
            .FirstOrDefaultAsync(cancellationToken);
    }


    public async Task<ICollection<BaseRefreshToken>> DeleteRangeAsync(ICollection<BaseRefreshToken> entities, bool permanent = false, CancellationToken cancellationToken = default)
    {
        if (!permanent)
        {
            foreach (var entity in entities)
            {
                entity.DeletedDate = DateTime.UtcNow;
                entity.IsActive = false;

            }
            Context.Set<RefreshToken>().UpdateRange((RefreshToken)entities);
        }
        else
        {
            Context.Set<RefreshToken>().RemoveRange((RefreshToken)entities);
        }

        await Context.SaveChangesAsync(cancellationToken);
        return entities;
    }

    //public async Task<List<RefreshToken>> GetOldRefreshTokensAsync(BaseUser user, string ipAdress)
    //{
    //    List<RefreshToken> tokens = await Query()
    //        .AsNoTracking()
    //        .Where(r =>
    //            r.UserId == user.Id
    //            && r.RevokedDate == null
    //            && r.ExpiresDate >= DateTime.UtcNow
    //            && r.CreatedByIp == ipAdress
    //        )
    //        .ToListAsync();

    //    return tokens;
    //}

    public async Task<IEnumerable<BaseRefreshToken>> GetOldRefreshTokensAsync(BaseUser user, string ipAdress)
    {
        return await Query()
            .AsNoTracking()
            .Where(r =>
                r.UserId == user.Id
                && r.RevokedDate == null
                && r.ExpiresDate >= DateTime.UtcNow
                && r.CreatedByIp == ipAdress
            )
            .ToListAsync();
    }
}
