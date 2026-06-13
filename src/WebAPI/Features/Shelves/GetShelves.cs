using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebAPI.Features.Common;

namespace WebAPI.Features.Shelves;

internal static class GetShelves
{
    internal sealed record Query : ListRequest;
    internal sealed record ShelfResponse(Guid Id, string Name, string? Description, int BookCount);

    internal sealed class Handler(AppDbContext db)
    {
        public async Task<PagedResult<ShelfResponse>> HandleAsync(Query query, CancellationToken ct)
        {
            return await db.Shelves
                .AsNoTracking()
                .OrderBy(s => s.Name)
                .ThenBy(s => s.Id)
                .Select(s => new ShelfResponse(
                    s.Id, s.Name, s.Description, s.Books.Count))
                .ToPagedResultAsync(query, ct);
        }
    }

    internal static async Task<Ok<Response<IReadOnlyList<ShelfResponse>>>> Endpoint(
        [AsParameters] Query query, 
        Handler handler, 
        HttpContext http, 
        CancellationToken ct)
    {
        var paged = await handler.HandleAsync(query, ct);
        return ApiResponse.Ok(paged, http);
    }
}