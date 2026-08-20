using Domain.Members;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations;

internal sealed class MemberConfiguration : AggregateRootConfiguration<Member>
{
	protected override void ConfigureEntity(EntityTypeBuilder<Member> builder)
	{
		builder.ToTable("members");

		builder.Property(m => m.Name)
			.HasMaxLength(Member.MaxNameLength)
			.HasColumnName("name");

		builder.Property(m => m.Email)
			.HasMaxLength(Member.MaxEmailLength)
			.HasColumnName("email");

		builder.Property(m => m.MembershipDate)
			.HasColumnName("membership_date");

		builder.HasIndex(m => m.Email)
			.HasDatabaseName("ix_members_email")
			.IsUnique();
	}
}