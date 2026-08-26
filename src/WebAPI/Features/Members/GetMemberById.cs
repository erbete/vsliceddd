using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database;
using Domain.Members;
using ErrorOr;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Features.Common;

namespace WebAPI.Features.Members;

internal static class GetMemberById
{
    internal sealed record Response(Guid Id, string Name, string Email, DateOnly MembershipDate);

    internal sealed class Handler(AppDbContext db)
    {
        public async Task<ErrorOr<Response>> HandleAsync(MemberId id, CancellationToken ct)
        {
            var member = await db.Members
                .AsNoTracking()
                .Where(m => m.Id == id)
                .Select(m => new Response(m.Id.Value, m.Name, m.Email, m.MembershipDate))
                .FirstOrDefaultAsync(ct);

            return member is null
                ? MemberErrors.NotFound(id)
                : member;
        }
    }

    internal static async Task<Results<Ok<Response>, ProblemHttpResult>> Endpoint(
        MemberId id,
        [FromServices] Handler handler,
        CancellationToken ct)
    {
        var result = await handler.HandleAsync(id, ct);

        return result.IsError
            ? Problems.From(result.FirstError)
            : TypedResults.Ok(result.Value);
    }
}