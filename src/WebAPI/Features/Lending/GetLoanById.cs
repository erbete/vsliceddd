using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Database;
using Domain.Lending;
using ErrorOr;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Infrastructure.Errors;

namespace WebAPI.Features.Lending;

internal static class GetLoanById
{
    internal sealed record Response(
        Guid Id,
        Guid CopyId,
        Guid MemberId,
        DateOnly LoanDate,
        DateOnly DueDate,
        DateOnly? ReturnDate);

    internal sealed class Handler(AppDbContext db)
    {
        public async Task<ErrorOr<Response>> HandleAsync(LoanId id, CancellationToken ct)
        {
            var loan = await db.Loans
                .AsNoTracking()
                .Where(l => l.Id == id)
                .Select(l => new Response(
                    l.Id.Value,
                    l.LendableCopyId.Value,
                    l.MemberId.Value,
                    l.LoanDate,
                    l.DueDate,
                    l.ReturnDate))
                .FirstOrDefaultAsync(ct);

            return loan is null
                ? LoanErrors.NotFound(id)
                : loan;
        }
    }

    internal static async Task<Results<Ok<Response>, ProblemHttpResult>> Endpoint(
        LoanId id,
        [FromServices] Handler handler,
        CancellationToken ct)
    {
        var result = await handler.HandleAsync(id, ct);

        return result.IsError
            ? Problems.From(result.FirstError)
            : TypedResults.Ok(result.Value);
    }
}