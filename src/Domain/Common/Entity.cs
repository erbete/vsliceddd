using System;

namespace Domain.Common;

public abstract class Entity
{
	public Guid Id { get; protected init; }
	public DateTimeOffset CreatedAt { get; }
	public DateTimeOffset UpdatedAt { get; }
}