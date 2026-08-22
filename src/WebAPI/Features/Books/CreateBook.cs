using System;
using System.Threading;
using System.Threading.Tasks;
using Database;
using Domain.Books;
using Domain.Common;
using EntityFramework.Exceptions.Common;
using ErrorOr;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebAPI.Features.Common;

namespace WebAPI.Features.Books;

internal static class CreateBook
{
    internal sealed record Request(string Title, int PublishedYear, Guid AuthorId, string? Isbn);
    internal sealed record Response(Guid Id, string Title, int PublishedYear, string? Isbn);

    internal sealed class CreateBookValidator : AbstractValidator<Request>
    {
        public CreateBookValidator()
        {
            RuleFor(b => b.Title)
                .NotEmpty()
                .MaximumLength(Book.MaxTitleLength);

            RuleFor(b => b.PublishedYear)
                .LessThan(DateTimeOffset.UtcNow.AddYears(1).Year)
                .GreaterThanOrEqualTo(Book.MinPublishedYear);

            RuleFor(b => b.AuthorId).NotEmpty();

            RuleFor(b => b.Isbn)
                .MaximumLength(Book.MaxIsbnLength)
                .When(b => b.Isbn is not null);
        }
    }

    internal sealed class Handler(AppDbContext db, IIdGenerator idGenerator)
    {
        public async Task<ErrorOr<Response>> HandleAsync(Request request, CancellationToken ct)
        {
            bool authorExists = await db.Authors.AnyAsync(a => a.Id == request.AuthorId, ct);
            if (!authorExists)
            {
                return BookErrors.AuthorNotFound(request.AuthorId);
            }

            var book = Book.Create(idGenerator.NewId(), request.Title, request.PublishedYear, request.AuthorId, request.Isbn);
            db.Books.Add(book);

            try
            {
                await db.SaveChangesAsync(ct);
            }
            catch (UniqueConstraintException)
            {
                return BookErrors.DuplicateIsbn(request.Title, request.Isbn);
            }

            return new Response(book.Id, book.Title, book.PublishedYear, book.Isbn);
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