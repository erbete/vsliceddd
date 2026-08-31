using System;
using Domain.Common;
using Domain.Members;
using ErrorOr;

namespace Domain.Lending;

public sealed class Loan : AggregateRoot<LoanId>
{
    public const int LoanPeriodDays = 14;

    public DateOnly LoanDate { get; private set; }
    public DateOnly DueDate { get; private set; }
    public DateOnly? ReturnDate { get; private set; }

    public LendableCopyId LendableCopyId { get; private set; }
    public MemberId MemberId { get; private set; }

    private Loan(DateOnly loanDate, LendableCopyId lendableCopyId, MemberId memberId)
    {
        GuardLoanDate(loanDate);

        Id = LoanId.New();
        LoanDate = loanDate;
        DueDate = loanDate.AddDays(LoanPeriodDays);
        LendableCopyId = lendableCopyId;
        MemberId = memberId;
    }

    public static Loan Create(
        DateOnly loanDate,
        LendableCopyId lendableCopyId,
        MemberId memberId) => new(loanDate, lendableCopyId, memberId);

    public ErrorOr<Success> MarkReturned(DateOnly returnDate)
    {
        if (ReturnDate is not null)
        {
            return LoanErrors.AlreadyReturned(Id, ReturnDate.Value);
        }

        if (returnDate < LoanDate)
        {
            return LoanErrors.ReturnDateBeforeLoanDate(returnDate, LoanDate);
        }

        if (returnDate > DateOnly.FromDateTime(DateTime.UtcNow))
        {
            return LoanErrors.ReturnDateInFuture(returnDate);
        }

        ReturnDate = returnDate;
        return Result.Success;
    }

    private static void GuardLoanDate(DateOnly loanDate) =>
        ArgumentOutOfRangeException.ThrowIfGreaterThan(loanDate, DateOnly.FromDateTime(DateTime.UtcNow));
}