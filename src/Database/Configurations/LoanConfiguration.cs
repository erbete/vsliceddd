using Domain.Books;
using Domain.Loans;
using Domain.Members;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations;

internal sealed class LoanConfiguration : AggregateRootConfiguration<Loan>
{
	protected override void ConfigureEntity(EntityTypeBuilder<Loan> builder)
	{
		builder.ToTable("loans");

		builder.Property(l => l.LoanDate)
			.HasColumnName("loan_date");

		builder.Property(l => l.DueDate)
			.HasColumnName("due_date");

		builder.Property(l => l.ReturnDate)
			.HasColumnName("return_date");

		builder.Property(l => l.BookItemId)
			.HasColumnName("book_item_id");

		builder.Property(l => l.MemberId)
			.HasColumnName("member_id");

		builder.HasOne<BookItem>()
			.WithMany()
			.HasForeignKey(l => l.BookItemId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.HasOne<Member>()
			.WithMany()
			.HasForeignKey(l => l.MemberId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.HasIndex(l => l.BookItemId)
			.IsUnique()
			.HasFilter("return_date IS NULL");
	}
}