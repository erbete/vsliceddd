using Domain.Authors;
using Domain.Books;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations;

internal sealed class BookConfiguration : AggregateRootConfiguration<Book, BookId>
{
	protected override void ConfigureEntity(EntityTypeBuilder<Book> builder)
	{
		builder.ToTable("books");

		builder.Property(p => p.Title)
			.HasColumnName("title")
			.HasMaxLength(Book.MaxTitleLength);

		builder.Property(b => b.Isbn)
			.HasColumnName("isbn")
			.HasMaxLength(Book.MaxIsbnLength);

		builder.Property(b => b.PublishedYear)
			.HasColumnName("published_year");

		builder.Property(b => b.AuthorId)
			.HasColumnName("author_id");

		builder.HasIndex(b => b.Isbn)
			.HasDatabaseName("ix_books_isbn")
			.IsUnique();

		builder.HasMany(b => b.BookItems)
			.WithOne(bi => bi.Book)
			.HasForeignKey(bi => bi.BookId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.Navigation(b => b.BookItems)
			.UsePropertyAccessMode(PropertyAccessMode.Field);

		builder.HasOne<Author>()
			.WithMany()
			.HasForeignKey(b => b.AuthorId)
			.OnDelete(DeleteBehavior.Restrict);
	}
}