using Domain.Shelves;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations;

internal sealed class ShelfConfiguration : AggregateRootConfiguration<Shelf>
{
	protected override void ConfigureEntity(EntityTypeBuilder<Shelf> builder)
	{
		builder.ToTable("shelves");

		builder.Property(p => p.Name)
			.HasColumnName("name")
			.HasMaxLength(Shelf.MaxNameLength)
			.IsRequired();

		builder.Property(p => p.Description)
			.HasColumnName("description")
			.HasMaxLength(Shelf.MaxDescriptionLength);

		builder.HasMany(s => s.Books)
			.WithOne()
			.HasForeignKey(b => b.ShelfId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.Navigation(s => s.Books)
			.UsePropertyAccessMode(PropertyAccessMode.Field);

		builder.HasIndex(s => s.Name)
			.HasDatabaseName("ix_shelves_name")
			.IsUnique();
	}
}