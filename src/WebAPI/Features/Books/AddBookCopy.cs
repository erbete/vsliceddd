using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database;
using Domain.Books;
using Domain.Lending;
using EntityFramework.Exceptions.Common;
using ErrorOr;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebAPI.Infrastructure.Errors;

namespace WebAPI.Features.Books;

internal static class AddBookCopy
{
    internal sealed record Request(string Barcode, DateOnly Acquired);
    internal sealed record Response(Guid Id, string Barcode, DateOnly Acquired);

    internal sealed class Validator : AbstractValidator<Request>
    {
        public Validator(TimeProvider timeProvider)
        {
            var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);

            RuleFor(r => r.Barcode)
                .NotEmpty()
                .MaximumLength(BookItem.MaxBarcodeLength);

            RuleFor(r => r.Acquired)
                .LessThanOrEqualTo(today)
                .WithMessage("Acquisition date cannot be in the future.");
        }
    }

    internal sealed class Handler(AppDbContext db)
    {
        public async Task<ErrorOr<Response>> HandleAsync(BookId bookId, Request request, CancellationToken ct)
        {
            var book = await db.Books
                .Include(b => b.BookItems)
                .FirstOrDefaultAsync(b => b.Id == bookId, ct);

            if (book is null)
            {
                return BookErrors.NotFound(bookId);
            }

            var result = book.AddCopy(request.Barcode, request.Acquired);
            if (result.IsError)
            {
                return result.FirstError;
            }

            var bookItemId = result.Value;
            db.LendableCopies.Add(LendableCopy.Create(bookItemId.Value, book.Id));

            try
            {
                await db.SaveChangesAsync(ct);
            }
            catch (UniqueConstraintException)
            {
                return BookErrors.DuplicateBarcode(request.Barcode);
            }

            var bookItem = book.BookItems.First(i => i.Id == bookItemId);
            return new Response(bookItem.Id.Value, bookItem.Barcode, bookItem.Acquired);
        }
    }

    internal static async Task<Results<Created<Response>, ProblemHttpResult>> Endpoint(
        BookId id,
        Request request,
        Handler handler,
        CancellationToken ct)
    {
        var result = await handler.HandleAsync(id, request, ct);

        return result.IsError
            ? Problems.From(result.FirstError)
            : TypedResults.Created($"/api/books/{id}/copies/{result.Value.Id}", result.Value);
    }
}