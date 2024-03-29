﻿//
// Generic Repository Implementation
//
// Copyright (C) 1995-2023, Yegor Mialyk. All Rights Reserved.
//
// Licensed under the MIT License. See the LICENSE file for details.
//

namespace My.Data.DomainModel;

public interface IEntityObject<TKey> where TKey : struct
{
    TKey Id { get; set; }
}
