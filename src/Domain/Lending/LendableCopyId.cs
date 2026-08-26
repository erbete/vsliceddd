using System;

namespace Domain.Lending;

public readonly record struct LendableCopyId : IParsable<LendableCopyId>
{
    public Guid Value { get; }

    private LendableCopyId(Guid value) => Value = value;

    public static LendableCopyId New() => new(Guid.CreateVersion7());

    public static LendableCopyId From(Guid value) =>
        value == Guid.Empty
        ? throw new ArgumentException("LendableCopyId cannot be empty.", nameof(value))
        : new LendableCopyId(value);

    public static LendableCopyId Parse(string s, IFormatProvider? provider) =>
        TryParse(s, provider, out var result)
            ? result
            : throw new FormatException($"'{s}' is not a valid LendableCopyId.");

    public static bool TryParse(string? s, IFormatProvider? provider, out LendableCopyId result)
    {
        if (Guid.TryParse(s, out var guid) && guid != Guid.Empty)
        {
            result = new LendableCopyId(guid);
            return true;
        }

        result = default;
        return false;
    }

    public override string ToString() => Value.ToString();
}