using System;

namespace Domain.Common;

public sealed record DateRange
{
	private DateRange(DateOnly start, DateOnly end)
	{
		Start = start;
		End = end;
	}

	public DateOnly Start { get; }
	public DateOnly End { get; }

	public static DateRange Create(DateOnly start, DateOnly end)
	{
		if (end < start) throw new ArgumentException("End cannot precede start.");
		return new DateRange(start, end);
	}
}