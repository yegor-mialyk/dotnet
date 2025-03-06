//
// Trim String Field Model Binder
//
// Copyright (C) 1995-2025, Yegor Mialyk. All Rights Reserved.
//
// Licensed under the MIT License. See the LICENSE file for details.
//

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace My.Web.Binders;

public sealed class TrimStringModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        return context.Metadata.ModelType == typeof(string) ? new TrimStringModelBinder() : null;
    }
}
