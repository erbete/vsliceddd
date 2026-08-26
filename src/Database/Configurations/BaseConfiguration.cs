using System;
using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations;

internal abstract class BaseConfiguration<T, TId> : IEntityTypeConfiguration<T>
	where T : Entity<TId>
	where TId : struct, IEquatable<TId>
{
	public virtual void Configure(EntityTypeBuilder<T> builder)
	{
		builder.HasKey(e => e.Id);

		builder.Property(e => e.Id)
			.HasColumnName("id")
			.ValueGeneratedNever();

		builder.Property(e => e.CreatedAt)
			.HasColumnName("created_at")
			.IsRequired();

		builder.Property(e => e.UpdatedAt)
			.HasColumnName("updated_at")
			.IsRequired();

		ConfigureEntity(builder);
	}

	protected abstract void ConfigureEntity(EntityTypeBuilder<T> builder);
}