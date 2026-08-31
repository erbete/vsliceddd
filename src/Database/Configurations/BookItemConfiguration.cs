using Domain.Books;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations;

internal sealed class BookItemConfiguration : BaseConfiguration<BookItem, BookItemId>
{
	protected override void ConfigureEntity(EntityTypeBuilder<BookItem> builder)
	{
		builder.ToTable("book_items", t =>
		{
			t.HasCheckConstraint(
				"ck_book_items_barcode_not_blank",
				"length(btrim(barcode)) > 0");
		});

		builder.Property(bi => bi.Barcode)
			.HasColumnName("barcode")
			.HasMaxLength(BookItem.MaxBarcodeLength)
			.IsRequired();

		builder.Property(bi => bi.Acquired)
			.HasColumnName("acquired");

		builder.Property(bi => bi.BookId)
			.HasColumnName("book_id");

		builder.HasIndex(bi => bi.Barcode)
			.HasDatabaseName("ix_book_items_barcode")
			.IsUnique();
	}
}