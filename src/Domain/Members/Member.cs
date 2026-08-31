using System;
using System.Globalization;
using Domain.Common;

namespace Domain.Members;

public sealed class Member : AggregateRoot<MemberId>
{
    public const int MaxNameLength = 255;
    public const int MaxEmailLength = 320;

    public string Name { get; private set; }
    public string Email { get; private set; }
    public DateOnly MembershipDate { get; private set; }

    private Member(string name, string email, DateOnly membershipDate)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(membershipDate, DateOnly.FromDateTime(DateTime.UtcNow));

        Id = MemberId.New();
        Name = GuardText(name, MaxNameLength, nameof(name));
        Email = GuardText(email, MaxEmailLength, nameof(email)).ToLower(CultureInfo.InvariantCulture);
        MembershipDate = membershipDate;
    }

    public static Member Create(
        string name,
        string email,
        DateOnly membershipDate) => new(name, email, membershipDate);

    private static string GuardText(string value, int maxLength, string paramName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, paramName);
        value = value.Trim();

        if (value.Length > maxLength)
        {
            throw new ArgumentException($"{paramName} exceeds maximum length of {maxLength} characters.", paramName);
        }

        return value;
    }
}