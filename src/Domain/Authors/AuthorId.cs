using System;

namespace Domain.Authors;

public readonly record struct AuthorId : IParsable<AuthorId>
{
    public Guid Value { get; }

    private AuthorId(Guid value) => Value = value;

    public static AuthorId New() => new(Guid.CreateVersion7());

    public static AuthorId From(Guid value) =>
        value == Guid.Empty
        ? throw new ArgumentException("AuthorId cannot be empty.", nameof(value))
        : new AuthorId(value);

    public static AuthorId Parse(string s, IFormatProvider? provider) =>
        TryParse(s, provider, out var result)
            ? result
            : throw new FormatException($"'{s}' is not a valid AuthorId.");

    public static bool TryParse(string? s, IFormatProvider? provider, out AuthorId result)
    {
        if (Guid.TryParse(s, out var guid) && guid != Guid.Empty)
        {
            result = new AuthorId(guid);
            return true;
        }

        result = default;
        return false;
    }

    public override string ToString() => Value.ToString();
}