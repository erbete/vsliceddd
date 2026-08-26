using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database;
using Domain.Authors;
using ErrorOr;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebAPI.Features.Common;

namespace WebAPI.Features.Authors;

internal static class GetAuthorById
{
    internal sealed record Response(Guid Id, string Name, string? Country);

    internal sealed class Handler(AppDbContext db)
    {
        public async Task<ErrorOr<Response>> HandleAsync(AuthorId id, CancellationToken ct)
        {
            var author = await db.Authors
                .AsNoTracking()
                .Where(a => a.Id == id)
                .Select(a => new Response(a.Id.Value, a.Name, a.Country))
                .FirstOrDefaultAsync(ct);

            if (author is null)
            {
                return AuthorErrors.NotFound(id);
            }

            return author;
        }
    }

    internal static async Task<Results<Ok<Response>, ProblemHttpResult>> Endpoint(
        AuthorId id,
        Handler handler,
        CancellationToken ct)
    {
        var result = await handler.HandleAsync(id, ct);

        return result.IsError
            ? Problems.From(result.FirstError)
            : TypedResults.Ok(result.Value);
    }
}