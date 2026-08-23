using System;
using Domain.Common;

namespace Domain.Authors;

public sealed class Author : AggregateRoot
{
    public const int MaxNameLength = 255;
    public const int MaxCountryLength = 255;

    public string Name { get; private set; }
    public string? Country { get; private set; }

    private Author(Guid id, string name, string? country)
    {
        name = name?.Trim()!;
        country = country?.Trim();

        GuardId(id);
        GuardName(name);
        GuardCountry(country);

        Id = id;
        Name = name;
        Country = country;
    }

    public static Author Create(Guid id, string name, string? country) => new(id, name, country);

    public void UpdateDetails(string name, string? country)
    {
        name = name?.Trim()!;
        country = country?.Trim();

        GuardName(name);
        GuardCountry(country);

        Name = name;
        Country = country;
    }

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

    private static void GuardCountry(string? country)
    {
        if (country is not null && country.Length > MaxCountryLength)
        {
            throw new ArgumentException($"Country exceeds maximum length of {MaxCountryLength} characters.");
        }
    }
}