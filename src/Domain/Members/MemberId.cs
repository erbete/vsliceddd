using System;

namespace Domain.Members;

public readonly record struct MemberId : IParsable<MemberId>
{
    public Guid Value { get; }

    private MemberId(Guid value) => Value = value;

    public static MemberId New() => new(Guid.CreateVersion7());

    public static MemberId From(Guid value) =>
        value == Guid.Empty
        ? throw new ArgumentException("MemberId cannot be empty.", nameof(value))
        : new MemberId(value);

    public static MemberId Parse(string s, IFormatProvider? provider) =>
        TryParse(s, provider, out var result)
            ? result
            : throw new FormatException($"'{s}' is not a valid MemberId.");

    public static bool TryParse(string? s, IFormatProvider? provider, out MemberId result)
    {
        if (Guid.TryParse(s, out var guid) && guid != Guid.Empty)
        {
            result = new MemberId(guid);
            return true;
        }

        result = default;
        return false;
    }

    public override string ToString() => Value.ToString();
}