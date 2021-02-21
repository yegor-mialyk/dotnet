//
// Generic Repository Implementation
//
// Copyright (C) 1995-2021, Yegor Mialyk. All Rights Reserved.
//
// Licensed under the MIT License. See the LICENSE file for details.
//

using System.Collections.Generic;
using My.Data.DomainModel;

namespace My.Data.Repository
{
    public interface IRepository<T> where T : class, IDomainObject
    {
        T GetById(int id);

        IEnumerable<T> Get();

        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Clear();
    }
}
