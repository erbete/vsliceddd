using System;
using ErrorOr;

namespace Domain.Lending;

public static class LoanErrors
{
    public static Error NotFound(LoanId id) =>
        Error.NotFound("Loan.NotFound", $"Loan {id} was not found.");

    public static Error AlreadyReturned(LoanId id, DateOnly returnDate) =>
        Error.Conflict("Loan.AlreadyReturned", $"Loan {id} was already returned on {returnDate:yyyy-MM-dd}.");

    public static Error ReturnDateBeforeLoanDate(DateOnly returnDate, DateOnly loanDate) =>
        Error.Validation("Loan.ReturnDateBeforeLoanDate", $"Return date {returnDate:yyyy-MM-dd} is before the loan date {loanDate:yyyy-MM-dd}.");

    public static Error ReturnDateInFuture(DateOnly returnDate) =>
        Error.Validation("Loan.ReturnDateInFuture", $"Return date {returnDate:yyyy-MM-dd} is in the future.");
}