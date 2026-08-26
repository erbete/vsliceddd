using Domain.Authors;
using Domain.Books;
using Domain.Lending;
using Domain.Members;
using Microsoft.EntityFrameworkCore;

namespace Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
	public DbSet<Author> Authors { get; set; }
	public DbSet<Book> Books { get; set; }
	public DbSet<Loan> Loans { get; set; }
	public DbSet<LendableCopy> LendableCopies { get; set; }
	public DbSet<Member> Members { get; set; }

	protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
	{
		configurationBuilder.Properties<AuthorId>().HaveConversion<AuthorIdConverter>();
		configurationBuilder.Properties<BookId>().HaveConversion<BookIdConverter>();
		configurationBuilder.Properties<BookItemId>().HaveConversion<BookItemIdConverter>();
		configurationBuilder.Properties<LendableCopyId>().HaveConversion<LendableCopyIdConverter>();
		configurationBuilder.Properties<LoanId>().HaveConversion<LoanIdConverter>();
		configurationBuilder.Properties<MemberId>().HaveConversion<MemberIdConverter>();
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
	}
}