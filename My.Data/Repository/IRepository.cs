//
// Generic Repository Implementation
//
// Copyright (C) 1995-2025, Yegor Mialyk. All Rights Reserved.
//
// Licensed under the MIT License. See the LICENSE file for details.
//

using Microsoft.EntityFrameworkCore.ChangeTracking;
using My.Data.DomainModel;

namespace My.Data.Repository;

public interface IRepository<TEntity, in TKey> where TEntity : class, IEntityObject<TKey> where TKey : struct
{
    TEntity? GetById(TKey id);

    ValueTask<TEntity?> GetByIdAsync(TKey id);

    IQueryable<TEntity> Get();

    EntityEntry<TEntity> Entry(TEntity entity);
    
    EntityEntry<TEntity> Add(TEntity entity);

    ValueTask<EntityEntry<TEntity>> AddAsync(TEntity entity);
    
    void Attach(TEntity entity);
    
    void AddRange(IEnumerable<TEntity> entities);

    Task AddRangeAsync(IEnumerable<TEntity> entities);

    void Update(TEntity entity);
    
    EntityEntry<TEntity> Delete(TEntity entity);

    void DeleteRange(IEnumerable<TEntity> entities);
}
