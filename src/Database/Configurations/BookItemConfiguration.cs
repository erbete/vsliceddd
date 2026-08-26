using Domain.Books;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations;

internal sealed class BookItemConfiguration : BaseConfiguration<BookItem, BookItemId>
{
	protected override void ConfigureEntity(EntityTypeBuilder<BookItem> builder)
	{
		builder.ToTable("book_items");

		builder.Property(bi => bi.Barcode)
			.HasColumnName("barcode")
			.HasMaxLength(BookItem.MaxBarcodeLength);

		builder.Property(bi => bi.BookId)
			.HasColumnName("book_id");

		builder.HasIndex(bi => bi.Barcode)
			.HasDatabaseName("ix_book_items_barcode")
			.IsUnique();
	}
}