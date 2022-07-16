//
// String Extensions
//
// Copyright (C) 1995-2022, Yegor Mialyk. All Rights Reserved.
//
// Licensed under the MIT License. See the LICENSE file for details.
//

using System.Diagnostics.CodeAnalysis;

namespace My.Common
{
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

        public static bool IsNullOrEmpty([NotNullWhen(false)] this string? value)
        {
            return string.IsNullOrEmpty(value);
        }
    }
}
