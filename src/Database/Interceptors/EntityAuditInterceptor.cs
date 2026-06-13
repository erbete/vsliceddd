using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Database.Interceptors;

public sealed class EntityAuditInterceptor(TimeProvider timeProvider) : SaveChangesInterceptor
{
	public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
	{
		UpdateTimestamps(eventData.Context);
		return base.SavingChanges(eventData, result);
	}

	public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
		DbContextEventData eventData,
		InterceptionResult<int> result,
		CancellationToken cancellationToken = default)
	{
		UpdateTimestamps(eventData.Context);
		return base.SavingChangesAsync(eventData, result, cancellationToken);
	}

	private void UpdateTimestamps(DbContext? context)
	{
		if (context == null) return;

		var now = timeProvider.GetUtcNow();
		var entries = context.ChangeTracker.Entries<Entity>();

		foreach (var entry in entries)
			switch (entry.State)
			{
				case EntityState.Added:
					entry.Property(nameof(Entity.CreatedAt)).CurrentValue = now;
					entry.Property(nameof(Entity.UpdatedAt)).CurrentValue = now;
					break;

				case EntityState.Modified:
					entry.Property(nameof(Entity.UpdatedAt)).CurrentValue = now;
					break;

				case EntityState.Detached:
				case EntityState.Unchanged:
				case EntityState.Deleted:
					break;
			}
	}
}