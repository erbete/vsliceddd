using System;
using System.Collections.Generic;
using Domain.Common;

namespace Domain.Books;

public sealed class Book : AggregateRoot
{
	public const int MaxTitleLength = 255;
	public const int MaxIsbnLength = 13;
	public const int MinPublishedYear = 1450;

	public string Title { get; private set; }
	public int PublishedYear { get; private set; }
	public string? Isbn { get; private set; }

	public Guid AuthorId { get; private set; }

	public IReadOnlyList<BookItem> BookItems => _items;
	private readonly List<BookItem> _items = [];

	private Book(Guid id, string title, int publishedYear, Guid authorId, string? isbn = null)
	{
		GuardTitle(title);
		GuardPublishedYear(publishedYear);
		GuardIsbn(isbn);
		GuardAuthorId(authorId);

		Id = id;
		Title = title.Trim();
		PublishedYear = publishedYear;
		Isbn = isbn?.Trim();
		AuthorId = authorId;
	}

	internal static Book Create(
		Guid id,
		string title,
		int publishedYear,
		Guid authorId,
		string? isbn = null) => new(id, title, publishedYear, authorId, isbn);

	// TODO add result pattern?
	public void UpdateDetails(string title, int publishedYear, string? isbn = null)
	{
		GuardTitle(title);
		GuardPublishedYear(publishedYear);
		GuardIsbn(isbn);

		Title = title.Trim();
		PublishedYear = publishedYear;
		Isbn = isbn?.Trim();
	}

	// TODO: add result pattern
	public void AddCopy(Guid id, string barcode, DateOnly acquired)
	{
		var item = BookItem.Create(id, barcode, acquired, this);
		_items.Add(item);
	}

	private static void GuardTitle(string title)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(title);

		if (title.Length > MaxTitleLength)
		{
			throw new ArgumentException($"Title exceeds maximum length of {MaxTitleLength} characters.");
		}
	}

	private static void GuardIsbn(string? isbn)
	{
		if (isbn is not null && isbn.Trim().Length > MaxIsbnLength)
		{
			throw new ArgumentException($"ISBN exceeds maximum length of {MaxIsbnLength} characters.");
		}
	}

	private static void GuardPublishedYear(int publishedYear)
	{
		ArgumentOutOfRangeException.ThrowIfGreaterThan(publishedYear, DateTimeOffset.UtcNow.Year);
		ArgumentOutOfRangeException.ThrowIfLessThan(publishedYear, MinPublishedYear);
	}

	private static void GuardAuthorId(Guid authorId)
	{
		if (authorId == Guid.Empty)
		{
			throw new ArgumentException("AuthorId cannot be empty.", nameof(authorId));
		}
	}
}