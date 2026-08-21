using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Database.Interceptors;

public sealed class DomainEventDispatchInterceptor(TimeProvider timeProvider) : SaveChangesInterceptor
{
    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        Dispatch(eventData.Context);
        return base.SavedChanges(eventData, result);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        await DispatchAsync(eventData.Context);
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private void Dispatch(DbContext? context) => DispatchAsync(context).GetAwaiter().GetResult();

    private Task DispatchAsync(DbContext? context)
    {
        if (context is null) return Task.CompletedTask;

        var aggregates = context.ChangeTracker
            .Entries<AggregateRoot>()
            .Select(e => e.Entity)
            .Where(a => a.DomainEvents.Count > 0)
            .ToList();

        var events = aggregates.SelectMany(a => a.DomainEvents).ToList();

        foreach (var aggregate in aggregates)
        {
            aggregate.ClearDomainEvents();
        }

        var now = timeProvider.GetUtcNow();
        foreach (var domainEvent in events)
        {
            domainEvent.OccurredAt = now;
            Console.WriteLine($"[DomainEvent] {domainEvent.GetType().Name} at {domainEvent.OccurredAt}: {domainEvent}");
        }

        return Task.CompletedTask;
    }
}