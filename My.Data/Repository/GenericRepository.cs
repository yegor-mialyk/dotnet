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
using Scm.Data.DomainModel;

namespace My.Data.Repository;

public class GenericRepository<TEntity, TKey, TDbContext> : IRepository<TEntity, TKey>
    where TEntity : class, IDomainObject<TKey>
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
        return DbSet.FirstOrDefault(entity => entity.Id.Equals(id));
    }
    
    public EntityEntry<TEntity> Entry(TEntity entity)
    {
        return DbContext.Entry(entity);
    }

    public IQueryable<TEntity> Get()
    {
        return DbSet;
    }

    public void Add(TEntity entity)
    {
        DbContext.Add(entity);
    }
    
    public void Attach(TEntity entity)
    {
        DbContext.Attach(entity);
    }

    public void AddRange(IEnumerable<TEntity> entities)
    {
        DbContext.AddRange(entities);
    }

    public void Update(TEntity entity)
    {
        if (EqualityComparer<TKey>.Default.Equals(entity.Id, default))
        {
            DbContext.Add(entity);
            return;
        }

        var entry = DbContext.Entry(entity);
        
        switch (entry.State)
        {
            case EntityState.Detached:
                DbContext.Attach(entity);
                entry.State = EntityState.Modified;
                return;
            case EntityState.Unchanged:
                entry.State = EntityState.Modified;
                break;
        }
    }

    public void Delete(TEntity entity)
    {
        DbContext.Remove(entity);
    }
}
