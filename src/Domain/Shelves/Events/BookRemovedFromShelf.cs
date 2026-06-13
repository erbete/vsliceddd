using System;
using Domain.Common;

namespace Domain.Shelves.Events;

public sealed record BookRemovedFromShelf(Guid ShelfId, Guid BookId) : IDomainEvent
{
    public DateTimeOffset OccurredAt { get; set; }
}