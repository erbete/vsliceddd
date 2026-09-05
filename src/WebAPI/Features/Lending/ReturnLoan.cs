using System;
using System.Threading;
using System.Threading.Tasks;
using Database;
using Domain.Lending;
using ErrorOr;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebAPI.Infrastructure.Errors;

namespace WebAPI.Features.Lending;

internal static class ReturnLoan
{
    internal sealed record Request(DateOnly ReturnDate);
    internal sealed record Response(
        Guid Id,
        Guid CopyId,
        DateOnly LoanDate,
        DateOnly DueDate,
        DateOnly ReturnDate);

    internal sealed class Validator : AbstractValidator<Request>
    {
        public Validator(TimeProvider timeProvider)
        {
            var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);

            RuleFor(r => r.ReturnDate)
                .LessThanOrEqualTo(today)
                    .WithMessage("Return date cannot be in the future.")
                .GreaterThanOrEqualTo(today.AddYears(-1))
                    .WithMessage("Return date is too far in the past.");
        }
    }

    internal sealed class Handler(AppDbContext db)
    {
        public async Task<ErrorOr<Response>> HandleAsync(LoanId id, Request request, CancellationToken ct)
        {
            var loan = await db.Loans.FirstOrDefaultAsync(l => l.Id == id, ct);
            if (loan is null)
            {
                return LoanErrors.NotFound(id);
            }

            var returned = loan.MarkReturned(request.ReturnDate);
            if (returned.IsError)
            {
                return returned.FirstError;
            }

            var copy = await db.LendableCopies.FirstOrDefaultAsync(c => c.Id == loan.LendableCopyId, ct);
            if (copy is null)
            {
                return LendingErrors.CopyNotFound(loan.LendableCopyId);
            }

            var checkedIn = copy.CheckIn();
            if (checkedIn.IsError)
            {
                return checkedIn.FirstError;
            }

            await db.SaveChangesAsync(ct);

            return new Response(loan.Id.Value, loan.LendableCopyId.Value, loan.LoanDate, loan.DueDate, loan.ReturnDate!.Value);
        }
    }

    internal static async Task<Results<Ok<Response>, ProblemHttpResult>> Endpoint(
        LoanId id,
        Request request,
        Handler handler,
        CancellationToken ct)
    {
        var result = await handler.HandleAsync(id, request, ct);

        return result.IsError
            ? Problems.From(result.FirstError)
            : TypedResults.Ok(result.Value);
    }
}