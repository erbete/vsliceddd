using Domain.Lending;
using Domain.Members;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations;

internal sealed class LoanConfiguration : AggregateRootConfiguration<Loan, LoanId>
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

		builder.Property(l => l.LendableCopyId)
			.HasColumnName("lendable_copy_id");

		builder.Property(l => l.MemberId)
			.HasColumnName("member_id");

		builder.HasOne<LendableCopy>()
			.WithMany()
			.HasForeignKey(l => l.LendableCopyId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.HasOne<Member>()
			.WithMany()
			.HasForeignKey(l => l.MemberId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.HasIndex(l => l.LendableCopyId)
			.IsUnique()
			.HasFilter("return_date IS NULL")
			.HasDatabaseName("ix_loans_lendable_copy_id_active");
	}
}