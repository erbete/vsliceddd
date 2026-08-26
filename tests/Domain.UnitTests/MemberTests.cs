using Domain.Members;
using Shouldly;

namespace Domain.UnitTests;

public sealed class MemberTests
{
    // private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.UtcNow);

    // [Fact]
    // public void Create_WithValidArguments_ShouldCreateMember()
    // {
    //     var id = Guid.NewGuid();
    //     var member = Member.Create(id, "John Doe", "john@example.com", Today);

    //     member.Id.ShouldBe(id);
    //     member.Name.ShouldBe("John Doe");
    //     member.Email.ShouldBe("john@example.com");
    //     member.MembershipDate.ShouldBe(Today);
    // }

    // [Fact]
    // public void Create_ShouldTrimNameAndNormalizeEmail()
    // {
    //     var member = Member.Create(Guid.NewGuid(), "  John Doe  ", "JOHN@Example.COM", Today);
    //     member.Name.ShouldBe("John Doe");
    //     member.Email.ShouldBe("john@example.com");
    // }

    // [Fact]
    // public void Create_WithEmptyId_ShouldThrowArgumentException()
    // {
    //     Should.Throw<ArgumentException>(() =>
    //         Member.Create(Guid.Empty, "John Doe", "john@example.com", Today)
    //     );
    // }

    // [Theory]
    // [InlineData(null)]
    // [InlineData("")]
    // [InlineData("   ")]
    // public void Create_WithInvalidName_ShouldThrowArgumentException(string? name)
    // {
    //     Should.Throw<ArgumentException>(() =>
    //         Member.Create(Guid.NewGuid(), name!, "john@example.com", Today)
    //     );
    // }

    // [Fact]
    // public void Create_WithTooLongName_ShouldThrowArgumentException()
    // {
    //     var name = new string('a', Member.MaxNameLength + 1);
    //     Should.Throw<ArgumentException>(() =>
    //         Member.Create(Guid.NewGuid(), name, "john@example.com", Today)
    //     );
    // }

    // [Theory]
    // [InlineData("invalid")]
    // [InlineData("invalid@")]
    // [InlineData("@example.com")]
    // [InlineData("john@")]
    // [InlineData("john@example")]
    // public void Create_WithInvalidEmail_ShouldThrowArgumentException(string email)
    // {
    //     Should.Throw<ArgumentException>(() =>
    //         Member.Create(Guid.NewGuid(), "John Doe", email, Today)
    //     );
    // }

    // [Fact]
    // public void Create_WithTooLongEmail_ShouldThrowArgumentException()
    // {
    //     var local = new string('a', Member.MaxEmailLength - "@example.com".Length + 1);
    //     var email = local + "@example.com";
    //     Should.Throw<ArgumentException>(() =>
    //         Member.Create(Guid.NewGuid(), "John Doe", email, Today)
    //     );
    // }

    // [Fact]
    // public void Create_WithFutureMembershipDate_ShouldThrowArgumentOutOfRangeException()
    // {
    //     var future = Today.AddDays(1);
    //     Should.Throw<ArgumentOutOfRangeException>(() =>
    //         Member.Create(Guid.NewGuid(), "John Doe", "john@example.com", future)
    //     );
    // }
}
