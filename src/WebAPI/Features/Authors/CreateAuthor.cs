using System;
using System.Threading;
using System.Threading.Tasks;
using Database;
using Domain.Authors;
using ErrorOr;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using WebAPI.Features.Common;

namespace WebAPI.Features.Authors;

internal static class CreateAuthor
{
    internal sealed record Request(string Name, string? Country);
    internal sealed record Response(Guid Id, string Name, string? Country);

    internal sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.Name)
                .NotEmpty()
                .MaximumLength(Author.MaxNameLength);

            RuleFor(r => r.Country)
                .MaximumLength(Author.MaxCountryLength);
        }
    }

    internal sealed class Handler(AppDbContext db)
    {
        public async Task<ErrorOr<Response>> HandleAsync(Request request, CancellationToken ct)
        {
            var author = Author.Create(request.Name, request.Country);
            db.Authors.Add(author);
            await db.SaveChangesAsync(ct);

            return new Response(author.Id.Value, author.Name, author.Country);
        }
    }

    internal static async Task<Results<CreatedAtRoute<Response>, ProblemHttpResult>> Endpoint(
        Request request,
        Handler handler,
        CancellationToken ct)
    {
        var result = await handler.HandleAsync(request, ct);

        return result.IsError
            ? Problems.From(result.FirstError)
            : TypedResults.CreatedAtRoute(result.Value, nameof(GetAuthorById), new { id = result.Value.Id });
    }
}