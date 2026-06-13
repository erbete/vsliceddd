using System;
using Domain.Common;

namespace Domain.Shelves.Events;

public sealed record BookAddedToShelf(Guid ShelfId, Guid BookId) : IDomainEvent
{
    public DateTimeOffset OccurredAt { get; set; }
}