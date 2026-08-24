using Domain.Authors;
using Shouldly;

namespace Domain.UnitTests;

public sealed class AuthorTests
{
    [Fact]
    public void Create_WithValidArguments_ShouldCreateAuthor()
    {
        var id = Guid.NewGuid();
        var author = Author.Create(id, "John Doe", "USA");

        author.Id.ShouldBe(id);
        author.Name.ShouldBe("John Doe");
        author.Country.ShouldBe("USA");
    }

    [Fact]
    public void Create_WithNullCountry_ShouldCreateAuthor()
    {
        var author = Author.Create(Guid.NewGuid(), "John Doe", null);
        author.Country.ShouldBeNull();
    }

    [Fact]
    public void Create_WithEmptyId_ShouldThrowArgumentException()
    {
        Should.Throw<ArgumentException>(() => Author.Create(Guid.Empty, "John Doe", "USA"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidName_ShouldThrowArgumentException(string? name)
    {
        Should.Throw<ArgumentException>(() => Author.Create(Guid.NewGuid(), name!, "USA"));
    }

    [Fact]
    public void Create_WithTooLongName_ShouldThrowArgumentException()
    {
        var name = new string('a', Author.MaxNameLength + 1);
        Should.Throw<ArgumentException>(() => Author.Create(Guid.NewGuid(), name, "USA"));
    }

    [Fact]
    public void Create_WithTooLongCountry_ShouldThrowArgumentException()
    {
        var country = new string('a', Author.MaxCountryLength + 1);
        Should.Throw<ArgumentException>(() => Author.Create(Guid.NewGuid(), "John Doe", country));
    }

    [Fact]
    public void Create_ShouldTrimNameAndCountry()
    {
        var author = Author.Create(Guid.NewGuid(), "  Alice  ", "  USA  ");
        author.Name.ShouldBe("Alice");
        author.Country.ShouldBe("USA");
    }

    [Fact]
    public void UpdateDetails_WithValidArguments_ShouldUpdate()
    {
        var author = Author.Create(Guid.NewGuid(), "John Doe", "USA");
        author.UpdateDetails("Jane Doe", "Canada");

        author.Name.ShouldBe("Jane Doe");
        author.Country.ShouldBe("Canada");
    }

    [Fact]
    public void UpdateDetails_WithTooLongName_ShouldThrowArgumentException()
    {
        var author = Author.Create(Guid.NewGuid(), "John Doe", "USA");
        var longName = new string('a', Author.MaxNameLength + 1);
        Should.Throw<ArgumentException>(() => author.UpdateDetails(longName, "USA"));
    }

    [Fact]
    public void UpdateDetails_ShouldTrimInputs()
    {
        var author = Author.Create(Guid.NewGuid(), "John Doe", "USA");
        author.UpdateDetails("  Bob  ", "  UK  ");
        author.Name.ShouldBe("Bob");
        author.Country.ShouldBe("UK");
    }
}
