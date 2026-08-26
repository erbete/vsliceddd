using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Authors;
using Domain.Books;
using Domain.Loans;
using Domain.Members;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

#pragma warning disable CA1848, CA1873

namespace Database.Seeding;

public sealed class LibrarySeeder(
    AppDbContext db,
    TimeProvider timeProvider,
    ILogger<LibrarySeeder> logger
)
{
    public async Task MigrateAndSeedAsync(CancellationToken ct)
    {
        logger.LogInformation("Seeding: migrating database");
        await db.Database.MigrateAsync(ct);
        await SeedIfEmptyAsync(ct);
    }

    public async Task MigrateAndSeedWithForceAsync(CancellationToken ct)
    {
        logger.LogInformation("Seeding: migrating database (force)");
        await db.Database.MigrateAsync(ct);
        await SeedAsync(force: true, ct);
    }

    public async Task SeedIfEmptyAsync(CancellationToken ct)
    {
        if (await db.Authors.AnyAsync(ct))
        {
            logger.LogInformation("Seeding: skipped — data exists");
            return;
        }

        await SeedAsync(force: false, ct);
    }

    public async Task SeedAsync(bool force, CancellationToken ct)
    {
        if (force)
        {
            logger.LogWarning("Seeding: force reseed — truncating existing data");
            await db.Loans.ExecuteDeleteAsync(ct);
            await db.Books.ExecuteDeleteAsync(ct);
            await db.Members.ExecuteDeleteAsync(ct);
            await db.Authors.ExecuteDeleteAsync(ct);
        }
        else if (await db.Authors.AnyAsync(ct))
        {
            logger.LogInformation("Seeding: skipped — data exists (use --seed --force to reseed)");
            return;
        }

        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);

        await using var tx = await db.Database.BeginTransactionAsync(ct);
        try
        {
            var authors = SeedData
                .Authors.Select(a => Author.Create(a.Id, a.Name, a.Country))
                .ToList();
            db.Authors.AddRange(authors);
            await db.SaveChangesAsync(ct);
            logger.LogInformation("Seeding: inserted {Count} authors", authors.Count);

            var books = new List<Book>();
            foreach (var b in SeedData.Books)
            {
                var book = Book.Create(b.Id, b.Title, b.PublishedYear, b.AuthorId, b.Isbn);
                foreach (var item in b.Items)
                {
                    var acquired = today.AddDays(-item.AcquiredDaysAgo);
                    var res = book.AddCopy(item.Id, item.Barcode, acquired);
                    if (res.IsError)
                    {
                        throw new InvalidOperationException(
                            $"Seed AddCopy failed book {b.Title} barcode {item.Barcode}: {res.FirstError.Code} {res.FirstError.Description}"
                        );
                    }
                }

                books.Add(book);
            }

            db.Books.AddRange(books);
            await db.SaveChangesAsync(ct);
            logger.LogInformation(
                "Seeding: inserted {Count} books with {ItemCount} copies",
                books.Count,
                books.Sum(b => b.BookItems.Count)
            );

            var members = SeedData
                .Members.Select(m =>
                {
                    var membershipDate = today.AddDays(-m.MembershipDaysAgo);
                    return Member.Create(m.Id, m.Name, m.Email, membershipDate);
                })
                .ToList();
            db.Members.AddRange(members);
            await db.SaveChangesAsync(ct);
            logger.LogInformation("Seeding: inserted {Count} members", members.Count);

            var loans = new List<Loan>();
            foreach (var l in SeedData.Loans)
            {
                var loanDate = today.AddDays(-l.LoanDaysAgo);
                var dueDate = loanDate.AddDays(l.DueDaysAfterLoan);
                var loan = Loan.Create(l.Id, loanDate, dueDate, l.BookItemId, l.MemberId);
                if (l.ReturnDaysAfterLoan is not null)
                {
                    var returnDate = loanDate.AddDays(l.ReturnDaysAfterLoan.Value);
                    loan.MarkReturned(returnDate);
                }

                loans.Add(loan);
            }

            db.Loans.AddRange(loans);
            await db.SaveChangesAsync(ct);
            logger.LogInformation(
                "Seeding: inserted {Count} loans ({Active} active)",
                loans.Count,
                loans.Count(x => x.ReturnDate is null)
            );

            await tx.CommitAsync(ct);
            logger.LogInformation("Seeding: complete");
        }
        catch (DbUpdateException ex)
        {
            await tx.RollbackAsync(ct);
            logger.LogError(ex, "Seeding: DB constraint violated — seed data bug");
            throw;
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync(ct);
            logger.LogError(ex, "Seeding: failed");
            throw;
        }
    }
}
