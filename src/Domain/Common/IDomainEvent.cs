using System;

namespace Domain.Common;

public interface IDomainEvent
{
	DateTimeOffset OccurredAt { get; set; }
}