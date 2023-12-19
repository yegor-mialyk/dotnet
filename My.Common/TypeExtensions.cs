//
// Type Extensions
//
// Copyright (C) 1995-2023, Yegor Mialyk. All Rights Reserved.
//
// Licensed under the MIT License. See the LICENSE file for details.
//

using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace BudgetInsights.Common;

public static class TypeExtensions
{
    public static bool IsNullableEnum(this Type type)
    {
        return type.IsGenericType &&
            type.GetGenericTypeDefinition() == typeof(Nullable<>) &&
            type.GetGenericArguments()[0].IsEnum;
    }

    public static bool IsNullableTypeOf<T>(this Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) &&
               type.GetGenericArguments()[0] == typeof(T);
    }

    public static IDictionary<string, string> ToDictionary(this Type type)
    {
        var dictionary = new Dictionary<string, string>();

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            type = type.GetGenericArguments()[0];
            dictionary.Add(string.Empty, "---- None ----");
        }

        var fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);

        foreach (var field in fields)
        {
            dictionary.Add(field.GetValue(type)?.ToString() ?? string.Empty,
                field.GetCustomAttributes<DisplayAttribute>(false).FirstOrDefault()?.Name ?? field.Name);
        }

        return dictionary;
    }
}
