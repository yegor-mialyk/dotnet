//
// Bootstrap Validation Summary with Success Message
//
// Copyright (C) 1995-2023, Yegor Mialyk. All Rights Reserved.
//
// Licensed under the MIT License. See the LICENSE file for details.
//

using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using My.Common;

namespace My.Web.Common;

public static class ValidationSummaryHelpers
{
    public static IHtmlContent ValidationSummaryEx(this IHtmlHelper htmlHelper)
    {
        if (htmlHelper.ViewData.ModelState.IsValid)
            return HtmlString.Empty;

        var sb = new StringBuilder();

        if (htmlHelper.ViewData.ModelState.TryGetValue("info", out var value))
            foreach (var message in value.Errors.Select(error => error.ErrorMessage).Where(s => !s.IsNullOrEmpty()))
                sb.AppendLine($@"<div class=""alert alert-success"" role=""alert"">{message}</div>");

        if (htmlHelper.ViewData.ModelState.TryGetValue(htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix, out value))
            foreach (var message in value.Errors.Select(error => error.ErrorMessage).Where(s => !s.IsNullOrEmpty()))
                sb.AppendLine($@"<div class=""alert alert-danger"" role=""alert"">{message}</div>");

        return new HtmlString(sb.ToString());
    }
}
