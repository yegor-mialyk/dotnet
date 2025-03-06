//
// Trim String Field Model Binder
//
// Copyright (C) 1995-2025, Yegor Mialyk. All Rights Reserved.
//
// Licensed under the MIT License. See the LICENSE file for details.
//

using Microsoft.AspNetCore.Mvc.ModelBinding;
using My.Common;

namespace My.Web.Binders;

public sealed class TrimStringModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var s = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).FirstValue;

        if (!s.IsNullOrEmpty())
            bindingContext.Result = ModelBindingResult.Success(s.Trim());

        return Task.CompletedTask;
    }
}
