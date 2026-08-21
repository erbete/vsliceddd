using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Common;
using ErrorOr;

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

	public IReadOnlyList<BookItem> BookItems => _bookItems;
	private readonly List<BookItem> _bookItems = [];

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

	public static Book Create(
		Guid id,
		string title,
		int publishedYear,
		Guid authorId,
		string? isbn = null) => new(id, title, publishedYear, authorId, isbn);

	public void UpdateDetails(string title, int publishedYear, string? isbn = null)
	{
		GuardTitle(title);
		GuardPublishedYear(publishedYear);
		GuardIsbn(isbn);

		Title = title.Trim();
		PublishedYear = publishedYear;
		Isbn = isbn?.Trim();
	}

	public ErrorOr<Success> UpdateBarcode(Guid bookItemId, string barcode)
	{
		var item = _bookItems.FirstOrDefault(i => i.Id == bookItemId);
		if (item is null)
		{
			return BookErrors.BookItemNotFound(bookItemId);
		}

		var trimmed = barcode.Trim();
		if (_bookItems.Any(i => i.Id != bookItemId && i.Barcode.Equals(trimmed, StringComparison.OrdinalIgnoreCase)))
		{
			return BookErrors.DuplicateBarcode(barcode);
		}

		item.UpdateBarcode(barcode);
		return Result.Success;
	}

	public ErrorOr<Guid> AddCopy(Guid id, string barcode, DateOnly acquired)
	{
		var trimmed = barcode.Trim();
		if (_bookItems.Any(i => i.Barcode.Equals(trimmed, StringComparison.OrdinalIgnoreCase)))
		{
			return BookErrors.DuplicateBarcode(barcode);
		}

		var item = BookItem.Create(id, barcode, acquired, this);
		_bookItems.Add(item);

		return item.Id;
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