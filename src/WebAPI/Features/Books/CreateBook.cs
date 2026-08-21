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
    internal sealed record Command(string Title, int PublishedYear, Guid AuthorId, string? Isbn);
    internal sealed record BookRequest(string Title, int PublishedYear, Guid AuthorId, string? Isbn);
    internal sealed record BookResponse(Guid Id, string Title, int PublishedYear, string? Isbn);

    internal sealed class CreateBookValidator : AbstractValidator<BookRequest>
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
        public async Task<ErrorOr<BookResponse>> HandleAsync(Command cmd, CancellationToken ct)
        {
            var authorExists = await db.Authors.AnyAsync(a => a.Id == cmd.AuthorId, ct);
            if (!authorExists)
            {
                return BookErrors.AuthorNotFound(cmd.AuthorId);
            }

            var book = Book.Create(idGenerator.NewId(), cmd.Title, cmd.PublishedYear, cmd.AuthorId, cmd.Isbn);
            db.Books.Add(book);

            try
            {
                await db.SaveChangesAsync(ct);
            }
            catch (UniqueConstraintException)
            {
                return BookErrors.DuplicateIsbn(cmd.Title, cmd.Isbn);
            }

            return new BookResponse(book.Id, book.Title, book.PublishedYear, book.Isbn);
        }
    }

    internal static async Task<Results<CreatedAtRoute<BookResponse>, ProblemHttpResult>> Endpoint(
        BookRequest request,
        Handler handler,
        CancellationToken ct)
    {
        var cmd = new Command(request.Title, request.PublishedYear, request.AuthorId, request.Isbn);
        var result = await handler.HandleAsync(cmd, ct);

        return result.IsError
            ? Problems.From(result.FirstError)
            : TypedResults.CreatedAtRoute(result.Value, nameof(GetBookById), new { id = result.Value.Id });
    }
}