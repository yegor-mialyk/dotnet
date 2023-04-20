//
// Type Extensions
//
// Copyright (C) 1995-2023, Yegor Mialyk. All Rights Reserved.
//
// Licensed under the MIT License. See the LICENSE file for details.
//

using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace My.Common;

public static class TypeExtensions
{
    public static bool IsNullableTypeOf<T>(this Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) &&
               type.GetGenericArguments()[0] == typeof(T);
    }

    public static IDictionary<string, string> ToDictionary<T>(this T type)
    {
        return typeof(T).GetFields(BindingFlags.Static | BindingFlags.Public)
            .ToDictionary(field => field.GetValue(type)?.ToString() ?? string.Empty,
                field => field.GetCustomAttributes<DisplayAttribute>(false).FirstOrDefault()?.Name ?? field.Name);
    }
}
