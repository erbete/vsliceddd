using Domain.Shelves;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations;

internal sealed class BookConfiguration : BaseConfiguration<Book>
{
	protected override void ConfigureEntity(EntityTypeBuilder<Book> builder)
	{
		builder.ToTable("books");

		builder.Property(p => p.Title)
			.HasColumnName("title")
			.HasMaxLength(Book.MaxTitleLength)
			.IsRequired();

		builder.Property(p => p.Author)
			.HasColumnName("author")
			.HasMaxLength(Book.MaxAuthorLength)
			.IsRequired();

		builder.Property(b => b.Isbn)
			.HasMaxLength(Book.MaxIsbnLength)
			.HasColumnName("isbn");

		builder.Property(b => b.ShelfId)
			.HasColumnName("shelf_id");

		builder.OwnsOne(b => b.ReadingPeriod, rp =>
		{
			rp.Property(r => r.Start).HasColumnName("reading_start");
			rp.Property(r => r.End).HasColumnName("reading_end");
		});

		builder.HasIndex(b => b.ShelfId)
			.HasDatabaseName("ix_books_shelf_id");

		builder.HasIndex(b => b.Isbn)
			.HasDatabaseName("ix_books_isbn");
	}
}