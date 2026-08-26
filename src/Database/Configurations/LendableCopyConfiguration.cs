using Domain.Lending;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations;

internal sealed class LendableCopyConfiguration : AggregateRootConfiguration<LendableCopy, LendableCopyId>
{
    protected override void ConfigureEntity(EntityTypeBuilder<LendableCopy> builder)
    {
        builder.ToTable("lendable_copies");

        builder.Property(c => c.BookId)
            .HasColumnName("book_id");

        builder.Property(c => c.CurrentLoanId)
            .HasColumnName("current_loan_id");

        builder.Ignore(c => c.IsAvailable);

        builder.HasIndex(c => new { c.BookId, c.CurrentLoanId })
            .HasDatabaseName("ix_lendable_copies_book_id_current_loan_id");
    }
}