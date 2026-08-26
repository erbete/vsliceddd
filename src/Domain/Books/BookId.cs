using System;

namespace Domain.Books;

public readonly record struct BookId : IParsable<BookId>
{
    public Guid Value { get; }

    private BookId(Guid value) => Value = value;

    public static BookId New() => new(Guid.CreateVersion7());

    public static BookId From(Guid value) =>
        value == Guid.Empty
        ? throw new ArgumentException("BookId cannot be empty.", nameof(value))
        : new BookId(value);

    public static BookId Parse(string s, IFormatProvider? provider) =>
        TryParse(s, provider, out var result)
            ? result
            : throw new FormatException($"'{s}' is not a valid BookId.");

    public static bool TryParse(string? s, IFormatProvider? provider, out BookId result)
    {
        if (Guid.TryParse(s, out var guid) && guid != Guid.Empty)
        {
            result = new BookId(guid);
            return true;
        }

        result = default;
        return false;
    }

    public override string ToString() => Value.ToString();
}