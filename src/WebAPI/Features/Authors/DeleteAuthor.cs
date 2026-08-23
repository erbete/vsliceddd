using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Features.Authors;

internal static class DeleteAuthor
{
    internal sealed class Handler(AppDbContext db)
    {
        public async Task HandleAsync(Guid id, CancellationToken ct)
        {
            await db.Authors
                .Where(a => a.Id == id)
                .ExecuteDeleteAsync(ct);
        }
    }

    internal static async Task<NoContent> Endpoint(
        Guid id,
        Handler handler,
        CancellationToken ct)
    {
        await handler.HandleAsync(id, ct);
        return TypedResults.NoContent();
    }
}