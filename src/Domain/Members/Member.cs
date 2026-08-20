using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Domain.Common;

namespace Domain.Members;

public sealed class Member : AggregateRoot
{
    public const int MaxNameLength = 255;
    public const int MaxEmailLength = 320;

    public string Name { get; private set; }
    public string Email { get; private set; }
    public DateOnly MembershipDate { get; private set; }

    private Member(Guid id, string name, string email, DateOnly membershipDate)
    {
        GuardId(id);
        GuardName(name);
        GuardEmail(email);
        GuardMembershipDate(membershipDate);

        Id = id;
        Name = name.Trim();
        Email = email.Trim().ToLower(CultureInfo.InvariantCulture);
        MembershipDate = membershipDate;
    }

    public static Member Create(
        Guid id,
        string name,
        string email,
        DateOnly membershipDate) => new(id, name, email, membershipDate);

    private static void GuardId(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id cannot be empty.", nameof(id));
        }
    }

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
            throw new ArgumentException($"Email exceeds maximum length of {MaxEmailLength} characters.");
        }

        var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        if (!Regex.IsMatch(email, emailRegex, RegexOptions.IgnoreCase))
        {
            throw new ArgumentException("Invalid email format.");
        }
    }

    private static void GuardMembershipDate(DateOnly membershipDate) =>
        ArgumentOutOfRangeException.ThrowIfGreaterThan(membershipDate, DateOnly.FromDateTime(DateTime.UtcNow));
}