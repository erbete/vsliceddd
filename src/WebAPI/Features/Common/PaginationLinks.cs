using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;

namespace WebAPI.Features.Common;

internal sealed record PaginationLinks(string First, string Last, string? Prev, string? Next)
{
    public static PaginationLinks Build(HttpRequest req, int page, int pageSize, int totalPages)
    {
        string Url(int p)
        {
            var q = QueryHelpers.ParseQuery(req.QueryString.Value ?? "");
            q["page"] = p.ToString(CultureInfo.InvariantCulture);
            q["pageSize"] = pageSize.ToString(CultureInfo.InvariantCulture);
            var qs = QueryString.Create(
                q.SelectMany(kv => kv.Value.Select(v =>
                    new KeyValuePair<string, string?>(kv.Key, v))));
            return $"{req.Scheme}://{req.Host}{req.PathBase}{req.Path}{qs}";
        }

        int last = totalPages < 1 ? 1 : totalPages;

        return new PaginationLinks(
            First: Url(1),
            Last:  Url(last),
            Prev:  page > 1 ? Url(page - 1) : null,
            Next:  page < totalPages ? Url(page + 1) : null);
    }
}