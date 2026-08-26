using System;

namespace Domain.Lending;

public readonly record struct LoanId : IParsable<LoanId>
{
    public Guid Value { get; }

    private LoanId(Guid value) => Value = value;

    public static LoanId New() => new(Guid.CreateVersion7());

    public static LoanId From(Guid value) =>
        value == Guid.Empty
        ? throw new ArgumentException("LoanId cannot be empty.", nameof(value))
        : new LoanId(value);

    public static LoanId Parse(string s, IFormatProvider? provider) => 
        TryParse(s, provider, out var result)
            ? result
            : throw new FormatException($"'{s}' is not a valid LoanId.");

    public static bool TryParse(string? s, IFormatProvider? provider, out LoanId result)
    {
        if (Guid.TryParse(s, out var guid) && guid != Guid.Empty)
        {
            result = new LoanId(guid);
            return true;
        }

        result = default;
        return false;
    }

    public override string ToString() => Value.ToString();
}