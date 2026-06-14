using System;
using Domain.Common;

namespace Domain.Shelves;

public sealed class Book : Entity
{
	public const int MaxTitleLength = 255;
	public const int MaxAuthorLength = 255;
	public const int MaxIsbnLength = 17;

	public string Title { get; private set; } = null!;
	public string Author { get; private set; } = null!;
	public string? Isbn { get; private set; }
	public DateRange? ReadingPeriod { get; private set; }

	public Guid ShelfId { get; private set; }

	private Book() { }

	internal static Book Create(Guid id, Guid shelfId, string title, string author, string? isbn = null)
	{
		GuardTitle(title);
		GuardAuthor(author);
		GuardIsbn(isbn);
		return new Book
		{
			Id = id,
			ShelfId = shelfId,
			Title = title.Trim(),
			Author = author.Trim(),
			Isbn = isbn?.Trim()
		};
	}

	internal void FinishReading(DateOnly startedOn, DateOnly finishedOn)
	{
		ReadingPeriod = DateRange.Create(startedOn, finishedOn);
	}

	internal void UpdateDetails(string title, string author, string? isbn = null)
	{
		GuardTitle(title);
		GuardAuthor(author);
		GuardIsbn(isbn);

		Title = title.Trim();
		Author = author.Trim();
		Isbn = isbn?.Trim();
	}

	private static void GuardTitle(string title)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(title);

		if (title.Length > MaxTitleLength)
		{
			throw new ArgumentException($"Title exceeds maximum length of {MaxTitleLength} characters.");
		}
	}

	private static void GuardAuthor(string author)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(author);

		if (author.Length > MaxAuthorLength)
		{
			throw new ArgumentException($"Author exceeds maximum length of {MaxAuthorLength} characters.");
		}
	}

	private static void GuardIsbn(string? isbn)
	{
		if (isbn is not null && isbn.Trim().Length > MaxIsbnLength)
		{
			throw new ArgumentException($"ISBN exceeds maximum length of {MaxIsbnLength} characters.");
		}
	}
}