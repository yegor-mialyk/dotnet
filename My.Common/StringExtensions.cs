﻿//
// String Extensions
//
// Copyright (C) 1995-2023, Yegor Mialyk. All Rights Reserved.
//
// Licensed under the MIT License. See the LICENSE file for details.
//

using System.Diagnostics.CodeAnalysis;

namespace My.Common;

public static class StringExtensions
{
    public static int ToInt32(this string? s, int defaultValue = 0)
    {
        return int.TryParse(s, out var result) ? result : defaultValue;
    }

    public static long ToInt64(this string? s, long defaultValue = 0)
    {
        return long.TryParse(s, out var result) ? result : defaultValue;
    }

    public static double ToDouble(this string? s, double defaultValue = 0)
    {
        return double.TryParse(s, out var result) ? result : defaultValue;
    }

    public static decimal ToDecimal(this string? s, decimal defaultValue = 0)
    {
        return decimal.TryParse(s, out var result) ? result : defaultValue;
    }

    public static bool IsNullOrEmpty([NotNullWhen(false)] this string? value)
    {
        return string.IsNullOrEmpty(value);
    }
}
