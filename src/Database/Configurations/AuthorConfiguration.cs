using Domain.Authors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations;

internal sealed class AuthorConfiguration : AggregateRootConfiguration<Author, AuthorId>
{
	protected override void ConfigureEntity(EntityTypeBuilder<Author> builder)
	{
		builder.ToTable("authors");

		builder.Property(a => a.Name)
			.HasColumnName("name")
			.HasMaxLength(Author.MaxNameLength);

		builder.Property(a => a.Country)
			.HasColumnName("country")
			.HasMaxLength(Author.MaxCountryLength);
	}
}