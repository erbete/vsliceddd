using System;
using System.Collections.Generic;

namespace Domain.Common;

public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot where TId : struct, IEquatable<TId>
{
	private readonly List<IDomainEvent> _domainEvents = [];
	public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents;

	protected void Raise(IDomainEvent domainEvent)
	{
		_domainEvents.Add(domainEvent);
	}

	public void ClearDomainEvents()
	{
		_domainEvents.Clear();
	}
}