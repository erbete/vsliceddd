using System;
using System.Threading;
using System.Threading.Tasks;
using Database;
using Domain.Common;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebAPI.Features.Common;

namespace WebAPI.Features.Shelves;

internal static class AddBookToShelf
{
    internal sealed record Request(string Title, string Author, string? Isbn);
    internal sealed record Command(Guid ShelfId, string Title, string Author, string? Isbn);
    internal sealed record BookResponse(Guid Id, string Title, string Author, string? Isbn);

    internal sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.Title).NotEmpty().MaximumLength(255);
            RuleFor(r => r.Author).NotEmpty().MaximumLength(255);
            RuleFor(r => r.Isbn).MaximumLength(17).When(r => r.Isbn is not null);
        }
    }

    internal sealed class Handler(AppDbContext db, IIdGenerator idGenerator)
    {
        public async Task<Result<Guid>> HandleAsync(Command cmd, CancellationToken ct)
        {
            var shelf = await db.Shelves
                .Include(s => s.Books)
                .FirstOrDefaultAsync(s => s.Id == cmd.ShelfId, ct);

            if (shelf is null)
            {
                return Result.Fail(ResultError.NotFound($"Shelf {cmd.ShelfId} not found."));
            }

            var result = shelf.AddBook(idGenerator.NewId(), cmd.Title, cmd.Author, cmd.Isbn);
            if (result.IsSuccess)
            {
                await db.SaveChangesAsync(ct);
            }

            return result;
        }
    }

    internal static async Task<Results<Created<Response<BookResponse>>, ProblemHttpResult>> Endpoint(
        Guid shelfId,
        Request request,
        Handler handler,
        HttpContext http,
        CancellationToken ct)
    {
        var result = await handler.HandleAsync(
            new Command(shelfId, request.Title, request.Author, request.Isbn), ct);

        if (!result.IsSuccess)
        {
            return ApiError.ToProblem(result.Error);
        }

        var book = new BookResponse(result.Value, request.Title, request.Author, request.Isbn);
        return ApiResponse.Created(book, http);
    }
}