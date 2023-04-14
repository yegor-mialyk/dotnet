//
// Async Helpers
//
// Copyright (C) 1995-2023, Yegor Mialyk. All Rights Reserved.
//
// Licensed under the MIT License. See the LICENSE file for details.
//

using System.Globalization;

namespace My.Common;

public static class AsyncHelpers
{
    private static readonly TaskFactory taskFactory = new(TaskScheduler.Default);

    public static TResult RunSync<TResult>(this Task<TResult> task, CancellationToken cancellationToken = default)
    {
        var currentCulture = CultureInfo.CurrentCulture;
        var currentUiCulture = CultureInfo.CurrentUICulture;

        return taskFactory.StartNew(() =>
        {
            Thread.CurrentThread.CurrentCulture = currentCulture;
            Thread.CurrentThread.CurrentUICulture = currentUiCulture;

            return task;
        }, cancellationToken).Unwrap().GetAwaiter().GetResult();
    }

    public static void RunSync(this Task task, CancellationToken cancellationToken = default)
    {
        var currentCulture = CultureInfo.CurrentCulture;
        var currentUiCulture = CultureInfo.CurrentUICulture;

        taskFactory.StartNew(() =>
        {
            Thread.CurrentThread.CurrentCulture = currentCulture;
            Thread.CurrentThread.CurrentUICulture = currentUiCulture;

            return task;
        }, cancellationToken).Unwrap().GetAwaiter().GetResult();
    }
}
