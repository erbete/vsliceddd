using System;
using System.Threading;
using System.Threading.Tasks;
using Database;
using Domain.Authors;
using Domain.Books;
using EntityFramework.Exceptions.Common;
using ErrorOr;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebAPI.Infrastructure.Errors;

namespace WebAPI.Features.Books;

internal static class CreateBook
{
    internal sealed record Request(string Title, int PublishedYear, Guid AuthorId, string? Isbn);
    internal sealed record Response(Guid Id, string Title, int PublishedYear, string? Isbn);

    internal sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.Title)
                .NotEmpty()
                .MaximumLength(Book.MaxTitleLength);

            RuleFor(r => r.PublishedYear)
                .LessThanOrEqualTo(DateTime.UtcNow.Year)
                .GreaterThanOrEqualTo(Book.MinPublishedYear);

            RuleFor(r => r.AuthorId)
                .NotEmpty();

            RuleFor(r => r.Isbn)
                .MaximumLength(Book.MaxIsbnLength);
        }
    }

    internal sealed class Handler(AppDbContext db)
    {
        public async Task<ErrorOr<Response>> HandleAsync(Request request, CancellationToken ct)
        {
            var authorId = AuthorId.From(request.AuthorId);

            bool authorExists = await db.Authors.AnyAsync(a => a.Id == authorId, ct);
            if (!authorExists)
            {
                return BookErrors.AuthorNotFound(authorId);
            }

            var book = Book.Create(request.Title, request.PublishedYear, authorId, request.Isbn);
            db.Books.Add(book);

            try
            {
                await db.SaveChangesAsync(ct);
            }
            catch (UniqueConstraintException)
            {
                return BookErrors.DuplicateIsbn(request.Title, request.Isbn);
            }

            return new Response(book.Id.Value, book.Title, book.PublishedYear, book.Isbn);
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
            : TypedResults.CreatedAtRoute(result.Value, nameof(GetBookById), new { id = result.Value.Id });
    }
}