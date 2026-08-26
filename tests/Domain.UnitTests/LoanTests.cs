using Domain.Lending;
using Shouldly;

namespace Domain.UnitTests;

public sealed class LoanTests
{
    // private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.UtcNow);
    // private static readonly DateOnly LoanDate = Today.AddDays(-10);
    // private static readonly DateOnly DueDate = Today.AddDays(-3);

    // private static Loan CreateLoan(DateOnly? loanDate = null, DateOnly? dueDate = null) =>
    //     Loan.Create(Guid.NewGuid(), loanDate ?? LoanDate, dueDate ?? DueDate, Guid.NewGuid(), Guid.NewGuid());

    // [Fact]
    // public void Create_WithValidArguments_ShouldCreateLoan()
    // {
    //     var id = Guid.NewGuid();
    //     var bookItemId = Guid.NewGuid();
    //     var memberId = Guid.NewGuid();

    //     var loan = Loan.Create(id, LoanDate, DueDate, bookItemId, memberId);

    //     loan.Id.ShouldBe(id);
    //     loan.LoanDate.ShouldBe(LoanDate);
    //     loan.DueDate.ShouldBe(DueDate);
    //     loan.BookItemId.ShouldBe(bookItemId);
    //     loan.MemberId.ShouldBe(memberId);
    //     loan.ReturnDate.ShouldBeNull();
    // }

    // [Fact]
    // public void Create_WithEmptyId_ShouldThrowArgumentException()
    // {
    //     Should.Throw<ArgumentException>(() =>
    //         Loan.Create(Guid.Empty, LoanDate, DueDate, Guid.NewGuid(), Guid.NewGuid())
    //     );
    // }

    // [Fact]
    // public void Create_WithEmptyBookItemId_ShouldThrowArgumentException()
    // {
    //     Should.Throw<ArgumentException>(() =>
    //         Loan.Create(Guid.NewGuid(), LoanDate, DueDate, Guid.Empty, Guid.NewGuid())
    //     );
    // }

    // [Fact]
    // public void Create_WithEmptyMemberId_ShouldThrowArgumentException()
    // {
    //     Should.Throw<ArgumentException>(() =>
    //         Loan.Create(Guid.NewGuid(), LoanDate, DueDate, Guid.NewGuid(), Guid.Empty)
    //     );
    // }

    // [Theory]
    // [InlineData(0)]
    // [InlineData(-1)]
    // public void Create_WithDueDateNotAfterLoanDate_ShouldThrowArgumentOutOfRangeException(int offsetDays)
    // {
    //     var loanDate = Today;
    //     var dueDate = Today.AddDays(offsetDays);
    //     Should.Throw<ArgumentOutOfRangeException>(() =>
    //         Loan.Create(Guid.NewGuid(), loanDate, dueDate, Guid.NewGuid(), Guid.NewGuid())
    //     );
    // }

    // [Fact]
    // public void MarkReturned_WithValidDate_ShouldSetReturnDate()
    // {
    //     var loan = CreateLoan();
    //     var returnDate = Today.AddDays(-2);

    //     loan.MarkReturned(returnDate);

    //     loan.ReturnDate.ShouldBe(returnDate);
    // }

    // [Fact]
    // public void MarkReturned_BeforeLoanDate_ShouldThrowArgumentOutOfRangeException()
    // {
    //     var loan = CreateLoan();
    //     Should.Throw<ArgumentOutOfRangeException>(() => loan.MarkReturned(LoanDate.AddDays(-1)));
    // }

    // [Fact]
    // public void MarkReturned_WhenAlreadyReturned_ShouldThrowInvalidOperationException()
    // {
    //     var loan = CreateLoan();
    //     loan.MarkReturned(Today.AddDays(-2));

    //     Should.Throw<InvalidOperationException>(() => loan.MarkReturned(Today.AddDays(-1)));
    // }
}
