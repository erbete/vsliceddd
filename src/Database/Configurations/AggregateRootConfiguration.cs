using System;
using Domain.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations;

internal abstract class AggregateRootConfiguration<T, TId> : BaseConfiguration<T, TId>
	where T : AggregateRoot<TId>
	where TId : struct, IEquatable<TId>
{
	public sealed override void Configure(EntityTypeBuilder<T> builder)
	{
		base.Configure(builder);
		builder.Ignore(e => e.DomainEvents);
	}
}