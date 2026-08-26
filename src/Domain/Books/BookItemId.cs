using System;

namespace Domain.Books;

public readonly record struct BookItemId : IParsable<BookItemId>
{
    public Guid Value { get; }

    private BookItemId(Guid value) => Value = value;

    public static BookItemId New() => new(Guid.CreateVersion7());

    public static BookItemId From(Guid value) =>
        value == Guid.Empty
        ? throw new ArgumentException("BookItemId cannot be empty.", nameof(value))
        : new BookItemId(value);

    public static BookItemId Parse(string s, IFormatProvider? provider) =>
        TryParse(s, provider, out var result)
            ? result
            : throw new FormatException($"'{s}' is not a valid BookItemId.");

    public static bool TryParse(string? s, IFormatProvider? provider, out BookItemId result)
    {
        if (Guid.TryParse(s, out var guid) && guid != Guid.Empty)
        {
            result = new BookItemId(guid);
            return true;
        }

        result = default;
        return false;
    }

    public override string ToString() => Value.ToString();
}