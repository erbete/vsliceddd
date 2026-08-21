using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database;
using Domain.Books;
using ErrorOr;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebAPI.Features.Common;

namespace WebAPI.Features.Books;

internal static class GetBookById
{
    internal sealed record Query(Guid Id);
    internal sealed record BookResponse(Guid Id, string Title, int PublishedYear, string? Isbn);

    internal sealed class Handler(AppDbContext db)
    {
        public async Task<ErrorOr<BookResponse>> HandleAsync(Query query, CancellationToken ct)
        {
            var book = await db.Books
                .AsNoTracking()
                .Where(b => b.Id == query.Id)
                .Select(b => new BookResponse(b.Id, b.Title, b.PublishedYear, b.Isbn))
                .FirstOrDefaultAsync(ct);

            if (book is null)
            {
                return BookErrors.NotFound(query.Id);
            }

            return book;
        }
    }

    internal static async Task<Results<Ok<BookResponse>, ProblemHttpResult>> Endpoint(
        Guid id,
        Handler handler,
        CancellationToken ct)
    {
        var result = await handler.HandleAsync(new Query(id), ct);

        return result.IsError
            ? Problems.From(result.FirstError)
            : TypedResults.Ok(result.Value);
    }
}