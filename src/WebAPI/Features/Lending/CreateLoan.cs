using System;
using System.Threading;
using System.Threading.Tasks;
using Database;
using Domain.Books;
using Domain.Lending;
using Domain.Members;
using EntityFramework.Exceptions.Common;
using ErrorOr;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebAPI.Features.Common;

namespace WebAPI.Features.Lending;

internal static class CreateLoan
{
    internal sealed record Request(Guid BookId, Guid MemberId);
    internal sealed record Response(Guid Id, Guid CopyId, DateOnly LoanDate, DateOnly DueDate);

    internal sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.BookId).NotEmpty();
            RuleFor(r => r.MemberId).NotEmpty();
        }
    }

    internal sealed class Handler(AppDbContext db, TimeProvider timeProvider)
    {
        public async Task<ErrorOr<Response>> HandleAsync(Request request, CancellationToken ct)
        {
            var bookId = BookId.From(request.BookId);
            var memberId = MemberId.From(request.MemberId);

            bool memberExists = await db.Members.AnyAsync(m => m.Id == memberId, ct);
            if (!memberExists)
            {
                return MemberErrors.NotFound(memberId);
            }

            var copy = await db.LendableCopies
                .FirstOrDefaultAsync(c => c.BookId == bookId && c.CurrentLoanId == null, ct);

            if (copy is null)
            {
                return LendingErrors.NoCopiesAvailable(bookId);
            }

            var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
            var loan = Loan.Create(today, copy.Id, memberId);

            var checkedOut = copy.CheckOut(loan.Id);
            if (checkedOut.IsError)
            {
                return checkedOut.FirstError;
            }

            db.Loans.Add(loan);

            try
            {
                await db.SaveChangesAsync(ct);
            }
            catch (UniqueConstraintException)
            {
                return LendingErrors.AlreadyOnLoan(copy.Id);
            }

            return new Response(loan.Id.Value, loan.LendableCopyId.Value, loan.LoanDate, loan.DueDate);
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
            : TypedResults.CreatedAtRoute(result.Value, nameof(GetLoanById), new { id = result.Value.Id });
    }
}