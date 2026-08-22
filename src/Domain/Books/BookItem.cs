using System;
using System.Globalization;
using Domain.Common;

namespace Domain.Books;

public sealed class BookItem : Entity
{
    public const int MaxBarcodeLength = 80;

    public string Barcode { get; private set; }
    public DateOnly Acquired { get; private set; }
    public Guid BookId { get; private set; }
    public Book Book { get; private set; } = null!;

    private BookItem(Guid id, string barcode, DateOnly acquired, Guid bookId)
    {
        barcode = Normalize(barcode);

        GuardId(id);
        GuardBarcode(barcode);
        GuardAcquired(acquired);
        GuardBookId(bookId);

        Id = id;
        Barcode = barcode;
        Acquired = acquired;
        BookId = bookId;
    }

    internal static BookItem Create(
        Guid id,
        string barcode,
        DateOnly acquired,
        Book book) => new(id, barcode, acquired, book.Id);

    internal void UpdateBarcode(string barcode)
    {
        barcode = Normalize(barcode);
        GuardBarcode(barcode);
        Barcode = barcode;
    }

    internal static string Normalize(string barcode)
        => barcode?.Trim().ToLower(CultureInfo.InvariantCulture)!;

    private static void GuardId(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id cannot be empty.", nameof(id));
        }
    }

    private static void GuardBarcode(string barcode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(barcode);
        if (barcode.Length > MaxBarcodeLength)
        {
            throw new ArgumentException($"Barcode exceeds maximum length of {MaxBarcodeLength} characters.", nameof(barcode));
        }
    }

    private static void GuardAcquired(DateOnly acquired)
        => ArgumentOutOfRangeException.ThrowIfGreaterThan(acquired, DateOnly.FromDateTime(DateTime.UtcNow));

    private static void GuardBookId(Guid bookId)
    {
        if (bookId == Guid.Empty)
        {
            throw new ArgumentException("BookId cannot be empty.", nameof(bookId));
        }
    }
}