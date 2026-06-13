using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database;
using Domain.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebAPI.Features.Common;

namespace WebAPI.Features.Shelves;

internal static class GetShelfById
{
    internal sealed record Query(Guid Id);

    internal sealed record ShelfResponse(
        Guid Id,
        string Name,
        string? Description,
        IReadOnlyList<BookResponse> Books);

    internal sealed record BookResponse(
        Guid Id,
        string Title,
        string Author,
        string? Isbn,
        DateOnly? ReadingStartedOn,
        DateOnly? ReadingFinishedOn);

    internal sealed class Handler(AppDbContext db)
    {
        public async Task<ShelfResponse?> HandleAsync(Query query, CancellationToken ct)
        {
            return await db.Shelves
                .AsNoTracking()
                .Where(s => s.Id == query.Id)
                .Select(s => new ShelfResponse(
                    s.Id, s.Name, s.Description,
                    s.Books
                        .OrderBy(b => b.Title)
                        .Select(b => new BookResponse(
                            b.Id, b.Title, b.Author, b.Isbn,
                            b.ReadingPeriod == null ? null : b.ReadingPeriod.Start,
                            b.ReadingPeriod == null ? null : b.ReadingPeriod.End))
                        .ToList()))
                .FirstOrDefaultAsync(ct);
        }
    }

    internal static async Task<Results<Ok<Response<ShelfResponse>>, ProblemHttpResult>> Endpoint(
        Guid id,
        Handler handler,
        HttpContext http,
        CancellationToken ct)
    {
        var shelf = await handler.HandleAsync(new Query(id), ct);

        return shelf is null
            ? ApiError.ToProblem(new ResultError(ErrorCode.NotFound, $"Shelf with ID {id} not found."))
            : ApiResponse.Ok(shelf, http);
    }
}