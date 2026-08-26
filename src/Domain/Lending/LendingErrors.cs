using Domain.Books;
using ErrorOr;

namespace Domain.Lending;

public static class LendingErrors
{
    public static Error CopyNotFound(LendableCopyId id) =>
        Error.NotFound("Lending.CopyNotFound", $"Copy {id} was not found.");

    public static Error AlreadyOnLoan(LendableCopyId id) =>
        Error.Conflict("Lending.AlreadyOnLoan", $"Copy {id} is already on loan.");

    public static Error NotOnLoan(LendableCopyId id) =>
        Error.Conflict("Lending.NotOnLoan", $"Copy {id} is not currently on loan.");

    public static Error NoCopiesAvailable(BookId id) =>
        Error.Conflict("Lending.NoCopiesAvailable", $"No copies of book {id} are available.");
}