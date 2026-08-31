using System;
using Domain.Books;
using Domain.Common;
using ErrorOr;

namespace Domain.Lending;

public sealed class LendableCopy : AggregateRoot<LendableCopyId>
{
    public BookId BookId { get; private set; }
    public LoanId? CurrentLoanId { get; private set; }
    public bool IsAvailable => CurrentLoanId is null;

    private LendableCopy(LendableCopyId id, BookId bookId)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(bookId, default);

        Id = id;
        BookId = bookId;
    }

    // lendableCopyId is the BookItem id
    public static LendableCopy Create(Guid lendableCopyId, BookId bookId) =>
        new(LendableCopyId.From(lendableCopyId), bookId);

    public ErrorOr<Success> CheckOut(LoanId loanId)
    {
        if (CurrentLoanId is not null)
        {
            return LendingErrors.AlreadyOnLoan(Id);
        }

        CurrentLoanId = loanId;
        return Result.Success;
    }

    public ErrorOr<Success> CheckIn()
    {
        if (CurrentLoanId is null)
        {
            return LendingErrors.NotOnLoan(Id);
        }

        CurrentLoanId = null;
        return Result.Success;
    }
}