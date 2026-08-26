using System;

namespace Domain.Common;

public abstract class Entity<TId> : IAuditable where TId : struct, IEquatable<TId>
{
	public TId Id { get; protected init; }
	public DateTimeOffset CreatedAt { get; }
	public DateTimeOffset UpdatedAt { get; }
}