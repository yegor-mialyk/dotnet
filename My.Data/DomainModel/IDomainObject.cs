//
// Generic Repository Implementation
//
// Copyright (C) 1995-2022, Yegor Mialyk. All Rights Reserved.
//
// Licensed under the MIT License. See the LICENSE file for details.
//

namespace My.Data.DomainModel
{
    public interface IDomainObject<TKey>
    {
        TKey Id { get; set; }
    }
}
