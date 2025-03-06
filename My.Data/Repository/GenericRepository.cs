//
// Generic Repository Implementation
//
// Copyright (C) 1995-2023, Yegor Mialyk. All Rights Reserved.
//
// Licensed under the MIT License. See the LICENSE file for details.
//

using EntityFrameworkCore.DbContextScope;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using My.Data.DomainModel;

namespace My.Data.Repository;

public class GenericRepository<TEntity, TKey, TDbContext> : IRepository<TEntity, TKey>
    where TEntity : class, IEntityObject<TKey>
    where TDbContext : DbContext
    where TKey : struct
{
    private readonly IAmbientDbContextLocator _ambientDbContextLocator;

    protected GenericRepository(IAmbientDbContextLocator ambientDbContextLocator)
    {
        _ambientDbContextLocator = ambientDbContextLocator;
    }

    protected TDbContext DbContext => _ambientDbContextLocator.Get<TDbContext>();

    protected DbSet<TEntity> DbSet => DbContext.Set<TEntity>();

    public TEntity? GetById(TKey id)
    {
        return DbSet.Find(id);
    }

    public ValueTask<TEntity?> GetByIdAsync(TKey id)
    {
        return DbSet.FindAsync(id);
    }
    
    public EntityEntry<TEntity> Entry(TEntity entity)
    {
        return DbContext.Entry(entity);
    }

    public IQueryable<TEntity> Get()
    {
        return DbSet;
    }

    public EntityEntry<TEntity> Add(TEntity entity)
    {
        return DbContext.Add(entity);
    }

    public ValueTask<EntityEntry<TEntity>> AddAsync(TEntity entity)
    {
        return DbContext.AddAsync(entity);
    }
    
    public void Attach(TEntity entity)
    {
        DbContext.Attach(entity);
    }

    public void AddRange(IEnumerable<TEntity> entities)
    {
        DbContext.AddRange(entities);
    }

    public Task AddRangeAsync(IEnumerable<TEntity> entities)
    {
        return DbContext.AddRangeAsync(entities);
    }

    public void Update(TEntity entity)
    {
        if (EqualityComparer<TKey>.Default.Equals(entity.Id, default))
        {
            DbContext.Add(entity);
            return;
        }

        DbContext.Entry(entity).State = EntityState.Modified;
    }

    public EntityEntry<TEntity> Delete(TEntity entity)
    {
        return DbContext.Remove(entity);
    }

    public void DeleteRange(IEnumerable<TEntity> entities)
    {
        DbContext.RemoveRange(entities);
    }
}
