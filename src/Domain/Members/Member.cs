using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Domain.Common;

namespace Domain.Members;

public sealed partial class Member : AggregateRoot<MemberId>
{
    public const int MaxNameLength = 255;
    public const int MaxEmailLength = 320;

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase)]
    private static partial Regex EmailRegex();

    public string Name { get; private set; }
    public string Email { get; private set; }
    public DateOnly MembershipDate { get; private set; }

    private Member(string name, string email, DateOnly membershipDate)
    {
        GuardName(name);
        GuardEmail(email);
        GuardMembershipDate(membershipDate);

        Id = MemberId.New();
        Name = name.Trim();
        Email = email.Trim().ToLower(CultureInfo.InvariantCulture);
        MembershipDate = membershipDate;
    }

    public static Member Create(
        string name,
        string email,
        DateOnly membershipDate) => new(name, email, membershipDate);

    private static void GuardName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (name.Length > MaxNameLength)
        {
            throw new ArgumentException($"Name exceeds maximum length of {MaxNameLength} characters.");
        }
    }

    private static void GuardEmail(string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        if (email.Length > MaxEmailLength)
        {
            throw new ArgumentException($"Email exceeds maximum length of {MaxEmailLength} characters.", nameof(email));
        }

        if (!EmailRegex().IsMatch(email))
        {
            throw new ArgumentException("Invalid email format.", nameof(email));
        }
    }

    private static void GuardMembershipDate(DateOnly membershipDate) =>
        ArgumentOutOfRangeException.ThrowIfGreaterThan(membershipDate, DateOnly.FromDateTime(DateTime.UtcNow));
}