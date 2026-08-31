using Domain.Authors;
using Domain.Books;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations;

internal sealed class BookConfiguration : AggregateRootConfiguration<Book, BookId>
{
	protected override void ConfigureEntity(EntityTypeBuilder<Book> builder)
	{
		builder.ToTable("books", t =>
		{
			t.HasCheckConstraint(
				"ck_books_published_year",
				$"published_year >= {Book.MinPublishedYear}");

			t.HasCheckConstraint(
				"ck_books_title_not_blank",
				"length(btrim(title)) > 0");

			t.HasCheckConstraint(
				"ck_books_isbn_not_blank",
				"isbn IS NULL OR length(btrim(isbn)) > 0");
		});

		builder.Property(p => p.Title)
			.HasColumnName("title")
			.IsRequired()
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
			.WithOne()
			.HasForeignKey(bi => bi.BookId)
			.HasConstraintName("fk_book_items_book_id")
			.OnDelete(DeleteBehavior.Cascade);

		builder.Navigation(b => b.BookItems)
			.UsePropertyAccessMode(PropertyAccessMode.Field);

		builder.HasOne<Author>()
			.WithMany()
			.HasForeignKey(b => b.AuthorId)
			.HasConstraintName("fk_books_author_id")
			.OnDelete(DeleteBehavior.Restrict);
	}
}