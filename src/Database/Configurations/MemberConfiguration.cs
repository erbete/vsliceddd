using Domain.Members;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations;

internal sealed class MemberConfiguration : AggregateRootConfiguration<Member, MemberId>
{
	protected override void ConfigureEntity(EntityTypeBuilder<Member> builder)
	{
		builder.ToTable("members", t =>
		{
			t.HasCheckConstraint("ck_members_name_not_blank", "length(btrim(name)) > 0");
			t.HasCheckConstraint("ck_members_email_not_blank", "length(btrim(email)) > 0");
		});

		builder.Property(m => m.Name)
			.HasMaxLength(Member.MaxNameLength)
			.IsRequired()
			.HasColumnName("name");

		builder.Property(m => m.Email)
			.HasMaxLength(Member.MaxEmailLength)
			.IsRequired()
			.HasColumnName("email");

		builder.Property(m => m.MembershipDate)
			.HasColumnName("membership_date");

		builder.HasIndex(m => m.Email)
			.HasDatabaseName("ix_members_email")
			.IsUnique();
	}
}