using System;
using System.Globalization;
using Domain.Common;

namespace Domain.Books;

public sealed class BookItem : Entity<BookItemId>
{
    public const int MaxBarcodeLength = 80;

    public string Barcode { get; private set; }
    public DateOnly Acquired { get; private set; }

    public BookId BookId { get; private set; }
    public Book Book { get; private set; } = null!;

    private BookItem(string barcode, DateOnly acquired, BookId bookId)
    {
        barcode = Normalize(barcode);

        GuardBarcode(barcode);
        GuardAcquired(acquired);

        Id = BookItemId.New();
        Barcode = barcode;
        Acquired = acquired;
        BookId = bookId;
    }

    internal static BookItem Create(
        string barcode,
        DateOnly acquired,
        BookId bookId) => new(barcode, acquired, bookId);

    internal void UpdateBarcode(string barcode)
    {
        barcode = Normalize(barcode);
        GuardBarcode(barcode);
        Barcode = barcode;
    }

    internal static string Normalize(string barcode) =>
        barcode?.Trim().ToLower(CultureInfo.InvariantCulture)!;

    private static void GuardBarcode(string barcode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(barcode);
        if (barcode.Length > MaxBarcodeLength)
        {
            throw new ArgumentException($"Barcode exceeds maximum length of {MaxBarcodeLength} characters.", nameof(barcode));
        }
    }

    private static void GuardAcquired(DateOnly acquired) =>
        ArgumentOutOfRangeException.ThrowIfGreaterThan(acquired, DateOnly.FromDateTime(DateTime.UtcNow));
}