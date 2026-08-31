using Domain.Authors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations;

internal sealed class AuthorConfiguration : AggregateRootConfiguration<Author, AuthorId>
{
	protected override void ConfigureEntity(EntityTypeBuilder<Author> builder)
	{
		builder.ToTable("authors", t =>
		{
			t.HasCheckConstraint(
				"ck_authors_name_not_blank",
				"length(btrim(name)) > 0");

			t.HasCheckConstraint(
				"ck_authors_country_not_blank",
				"country IS NULL OR length(btrim(country)) > 0");
		});

		builder.Property(a => a.Name)
			.HasColumnName("name")
			.HasMaxLength(Author.MaxNameLength)
			.IsRequired();

		builder.Property(a => a.Country)
			.HasColumnName("country")
			.HasMaxLength(Author.MaxCountryLength);
	}
}