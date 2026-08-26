using System;
using Domain.Common;

namespace Domain.Authors;

public sealed class Author : AggregateRoot<AuthorId>
{
    public const int MaxNameLength = 255;
    public const int MaxCountryLength = 255;

    public string Name { get; private set; }
    public string? Country { get; private set; }

    private Author(string name, string? country)
    {
        name = name?.Trim()!;
        country = country?.Trim();

        GuardName(name);
        GuardCountry(country);

        Id = AuthorId.New();
        Name = name;
        Country = country;
    }

    public static Author Create(string name, string? country) => new(name, country);

    public void UpdateDetails(string name, string? country)
    {
        name = name?.Trim()!;
        country = country?.Trim();

        GuardName(name);
        GuardCountry(country);

        Name = name;
        Country = country;
    }

    private static void GuardName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (name.Length > MaxNameLength)
        {
            throw new ArgumentException($"Name exceeds maximum length of {MaxNameLength} characters.");
        }
    }

    private static void GuardCountry(string? country)
    {
        if (country is not null && country.Length > MaxCountryLength)
        {
            throw new ArgumentException($"Country exceeds maximum length of {MaxCountryLength} characters.");
        }
    }
}