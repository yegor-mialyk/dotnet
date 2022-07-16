//
// Generic Repository Implementation
//
// Copyright (C) 1995-2022, Yegor Mialyk. All Rights Reserved.
//
// Licensed under the MIT License. See the LICENSE file for details.
//

using EntityFrameworkCore.DbContextScope;
using Microsoft.EntityFrameworkCore;
using My.Data.DomainModel;

namespace My.Data.Repository
{
    public class GenericRepository<TEntity, TKey, TDbContext> : IRepository<TEntity, TKey>
        where TEntity : class, IDomainObject<TKey> where TDbContext : DbContext
    {
        private readonly IAmbientDbContextLocator _ambientDbContextLocator;

        protected GenericRepository(IAmbientDbContextLocator ambientDbContextLocator)
        {
            _ambientDbContextLocator = ambientDbContextLocator;
        }

        protected TDbContext DbContext => _ambientDbContextLocator.Get<TDbContext>();

        protected virtual DbSet<TEntity> DbSet => DbContext.Set<TEntity>();

        public virtual TEntity? GetById(TKey id)
        {
            return DbSet.Find(id);
        }

        public virtual IEnumerable<TEntity> Get()
        {
            return DbSet;
        }

        public virtual void Add(TEntity entity)
        {
            DbSet.Add(entity);
        }

        public virtual void AddRange(IEnumerable<TEntity> entities)
        {
            DbSet.AddRange(entities);
        }

        public virtual void Update(TEntity entity)
        {
            if (EqualityComparer<TKey>.Default.Equals(entity.Id, default))
            {
                Add(entity);
                return;
            }

            if (DbContext.Entry(entity).State == EntityState.Detached)
                DbSet.Attach(entity);

            DbContext.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Delete(TEntity entity)
        {
            if (DbContext.Entry(entity).State == EntityState.Detached)
                DbSet.Attach(entity);

            DbSet.Remove(entity);
        }
    }
}
