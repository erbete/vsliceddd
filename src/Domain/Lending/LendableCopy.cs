using Domain.Books;
using Domain.Common;
using ErrorOr;

namespace Domain.Lending;

public sealed class LendableCopy : AggregateRoot<LendableCopyId>
{
    public BookId BookId { get; private set; }
    public LoanId? CurrentLoanId { get; private set; }
    public bool IsAvailable => CurrentLoanId is null;

    private LendableCopy(BookId bookId)
    {
        Id = LendableCopyId.New();
        BookId = bookId;
    }

    public static LendableCopy Create(BookId bookId) => new(bookId);

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