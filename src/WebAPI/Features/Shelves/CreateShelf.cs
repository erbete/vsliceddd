using System;
using System.Threading;
using System.Threading.Tasks;
using Database;
using Domain.Common;
using Domain.Shelves;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebAPI.Features.Common;

namespace WebAPI.Features.Shelves;

internal static class CreateShelf
{
    internal sealed record Request(string Name, string? Description);
    internal sealed record Command(string Name, string? Description);
    internal sealed record ShelfResponse(Guid Id, string Name, string? Description);

    internal sealed class CreateShelfValidator : AbstractValidator<Request>
    {
        public CreateShelfValidator()
        {
            RuleFor(r => r.Name)
                .NotEmpty()
                .MaximumLength(255);

            RuleFor(r => r.Description)
                .MaximumLength(1000)
                .When(r => r.Description is not null);
        }
    }

    internal sealed class Handler(AppDbContext db, IIdGenerator idGenerator)
    {
        public async Task<Result<ShelfResponse>> HandleAsync(Command cmd, CancellationToken ct)
        {
            var exists = await db.Shelves.AnyAsync(s => s.Name == cmd.Name, ct);
            if (exists)
            {
                return Result.Fail(ResultError.Conflict($"Shelf '{cmd.Name}' already exists."));
            }

            var shelf = Shelf.Create(idGenerator.NewId(), cmd.Name, cmd.Description);
            db.Shelves.Add(shelf);

            await db.SaveChangesAsync(ct);

            return Result<ShelfResponse>
                .Success(new ShelfResponse(shelf.Id, shelf.Name, shelf.Description));
        }
    }

    internal static async Task<Results<CreatedAtRoute<Response<ShelfResponse>>, ProblemHttpResult>> Endpoint(
        Request request,
        Handler handler,
        HttpContext http,
        CancellationToken ct)
    {
        var result = await handler.HandleAsync(new Command(request.Name, request.Description), ct);
        return result.IsSuccess
            ? ApiResponse.CreatedAtRoute(result.Value, nameof(GetShelfById), new { id = result.Value.Id }, http)
            : ApiError.ToProblem(result.Error);
    }
}