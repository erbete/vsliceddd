using System;
using Domain.Common;

namespace Domain.Loans;

public sealed class Loan : AggregateRoot
{
    public DateOnly LoanDate { get; private set; }
    public DateOnly DueDate { get; private set; }
    public DateOnly? ReturnDate { get; private set; }

    public Guid BookItemId { get; private set; }
    public Guid MemberId { get; private set; }

    private Loan(Guid id, DateOnly loanDate, DateOnly dueDate, Guid bookItemId, Guid memberId)
    {
        GuardId(id);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(dueDate, loanDate);
        GuardBookItemId(bookItemId);
        GuardMemberId(memberId);

        Id = id;
        LoanDate = loanDate;
        DueDate = dueDate;
        BookItemId = bookItemId;
        MemberId = memberId;
    }

    public static Loan Create(
        Guid id,
        DateOnly loanDate,
        DateOnly dueDate,
        Guid bookItemId,
        Guid memberId) => new(id, loanDate, dueDate, bookItemId, memberId);

    public void MarkReturned(DateOnly returnDate)
    {
        if (ReturnDate is not null)
        {
            throw new InvalidOperationException("Loan is already returned.");
        }

        ArgumentOutOfRangeException.ThrowIfLessThan(returnDate, LoanDate);

        ReturnDate = returnDate;
    }

    private static void GuardId(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id cannot be empty.", nameof(id));
        }
    }

    private static void GuardBookItemId(Guid bookItemId)
    {
        if (bookItemId == Guid.Empty)
        {
            throw new ArgumentException("BookItemId cannot be empty.", nameof(bookItemId));
        }
    }

    private static void GuardMemberId(Guid memberId)
    {
        if (memberId == Guid.Empty)
        {
            throw new ArgumentException("MemberId cannot be empty.", nameof(memberId));
        }
    }
}