using Domain.Common;
using Shouldly;

namespace Domain.UnitTests;

public sealed class DateRangeTests
{
    [Fact]
    public void Create_WithValidRange_ShouldCreateDateRange()
    {
        var start = new DateOnly(2024, 1, 1);
        var end = new DateOnly(2024, 12, 31);

        var range = DateRange.Create(start, end);

        range.Start.ShouldBe(start);
        range.End.ShouldBe(end);
    }

    [Fact]
    public void Create_WithSameStartAndEnd_ShouldCreateDateRange()
    {
        var date = new DateOnly(2024, 6, 15);
        var range = DateRange.Create(date, date);
        range.Start.ShouldBe(date);
        range.End.ShouldBe(date);
    }

    [Fact]
    public void Create_WithEndBeforeStart_ShouldThrowArgumentException()
    {
        var start = new DateOnly(2024, 12, 31);
        var end = new DateOnly(2024, 1, 1);

        Should.Throw<ArgumentException>(() => DateRange.Create(start, end));
    }
}
