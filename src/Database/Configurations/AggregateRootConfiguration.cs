using Domain.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations;

internal abstract class AggregateRootConfiguration<T> : BaseConfiguration<T> where T : AggregateRoot
{
	public sealed override void Configure(EntityTypeBuilder<T> builder)
	{
		base.Configure(builder);
		builder.Ignore(e => e.DomainEvents);
	}
}