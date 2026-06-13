using System;
using System.Threading;
using System.Threading.Tasks;
using Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Features.Shelves;

internal static class DeleteShelf
{
    internal sealed record Command(Guid Id);

    internal sealed class Handler(AppDbContext db)
    {
        public async Task HandleAsync(Command cmd, CancellationToken ct)
        {
            var shelf = await db.Shelves.FirstOrDefaultAsync(s => s.Id == cmd.Id, ct);

            if (shelf is null)
            {
                return;
            }

            db.Shelves.Remove(shelf);
            await db.SaveChangesAsync(ct);
        }
    }

    internal static async Task<NoContent> Endpoint(Guid id, Handler handler, CancellationToken ct)
    {
        await handler.HandleAsync(new Command(id), ct);
        return TypedResults.NoContent();
    }
}