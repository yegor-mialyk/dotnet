//
// Generic Repository Implementation
//
// Copyright (C) 1995-2021, Yegor Mialyk. All Rights Reserved.
//
// Licensed under the MIT License. See the LICENSE file for details.
//

using System;
using System.Collections.Generic;
using My.Data.DomainModel;
using EntityFrameworkCore.DbContextScope;
using Microsoft.EntityFrameworkCore;

namespace My.Data.Repository
{
    public class GenericRepository<T, TDbContext> : IRepository<T> where T : class, IDomainObject where TDbContext : DbContext
    {
        private readonly IAmbientDbContextLocator _ambientDbContextLocator;

        public GenericRepository(IAmbientDbContextLocator ambientDbContextLocator)
        {
            _ambientDbContextLocator = ambientDbContextLocator;
        }

        protected TDbContext DbContext
        {
            get
            {
                var dbContext = _ambientDbContextLocator.Get<TDbContext>();

                if (dbContext == null)
                    throw new InvalidOperationException("No ambient DbContext found. The repository method has been called outside of the DbContextScope.");

                return dbContext;
            }
        }

        protected virtual DbSet<T> DbSet => DbContext.Set<T>();

        public virtual T GetById(int id)
        {
            return DbSet.Find(id);
        }

        public virtual void Clear()
        {
            DbSet.RemoveRange(DbSet);
        }

        public virtual IEnumerable<T> Get()
        {
            return DbSet;
        }

        public virtual void Add(T entity)
        {
            DbSet.Add(entity);
        }

        public virtual void Update(T entity)
        {
            if (entity.Id == 0)
            {
                Add(entity);
                return;
            }

            if (DbContext.Entry(entity).State == EntityState.Detached)
                DbSet.Attach(entity);

            DbContext.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Delete(T entity)
        {
            if (DbContext.Entry(entity).State == EntityState.Detached)
                DbSet.Attach(entity);

            DbSet.Remove(entity);
        }
    }
}
