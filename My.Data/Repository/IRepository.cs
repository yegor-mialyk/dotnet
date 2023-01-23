//
// Generic Repository Implementation
//
// Copyright (C) 1995-2023, Yegor Mialyk. All Rights Reserved.
//
// Licensed under the MIT License. See the LICENSE file for details.
//

using Microsoft.EntityFrameworkCore.ChangeTracking;
using My.Data.DomainModel;

namespace My.Data.Repository;

public interface IRepository<TEntity, in TKey> where TEntity : class, IDomainObject<TKey> where TKey : struct
{
    TEntity? GetById(TKey id);

    IQueryable<TEntity> Get();

    EntityEntry<TEntity> Entry(TEntity entity);
    
    void Add(TEntity entity);
    
    void Attach(TEntity entity);
    
    void AddRange(IEnumerable<TEntity> entities);

    void Update(TEntity entity);
    
    void Delete(TEntity entity);
}
