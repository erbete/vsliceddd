using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Common;
using Domain.Shelves.Events;

namespace Domain.Shelves;

public sealed class Shelf : AggregateRoot
{
	public const int MaxNameLength = 255;
	public const int MaxDescriptionLength = 2000;
	public const int MaxBooks = 100;

	public string Name { get; private set; } = null!;
	public string? Description { get; private set; }

	public IReadOnlyList<Book> Books => _books;
	private readonly List<Book> _books = [];

	private Shelf() { }

	public static Shelf Create(Guid id, string name, string? description = null)
	{
		GuardName(name);
		GuardDescription(description);

		return new Shelf
		{
			Id = id,
			Name = name.Trim(),
			Description = description?.Trim()
		};
	}

	public Result<Guid> AddBook(Guid bookId, string title, string author, string? isbn = null)
	{
		if (_books.Count >= MaxBooks)
		{
			return Result.Fail(ResultError.Conflict($"Shelf is full (max {MaxBooks} books)."));
		}

		var book = Book.Create(bookId, title, author, isbn);
		book.AttachToShelf(Id);
		_books.Add(book);
		Raise(new BookAddedToShelf(Id, book.Id));
		return Result<Guid>.Success(book.Id);
	}

	public void RemoveBook(Guid bookId)
	{
		var book = FindBook(bookId);
		if (book is null) return;
		_books.Remove(book);
		Raise(new BookRemovedFromShelf(Id, book.Id));
	}

	public Result FinishReadingBook(Guid bookId, DateOnly startedOn, DateOnly finishedOn)
	{
		var book = FindBook(bookId);
		if (book is null)
		{
			return Result.Fail(ResultError.NotFound("Book not on shelf."));
		}

		book.FinishReading(startedOn, finishedOn);
		return Result.Success();
	}

	public Result UpdateBookDetails(Guid bookId, string title, string author, string? isbn = null)
	{
		var book = FindBook(bookId);
		if (book is null)
		{
			return Result.Fail(ResultError.NotFound("Book not on shelf."));
		}

		book.UpdateDetails(title, author, isbn);
		return Result.Success();
	}

	public void Rename(string name)
	{
		GuardName(name);
		Name = name.Trim();
	}

	public void UpdateDescription(string? description)
	{
		GuardDescription(description);
		Description = description?.Trim();
	}

	private Book? FindBook(Guid bookId) => _books.SingleOrDefault(b => b.Id == bookId);

	private static void GuardName(string name)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(name);

		if (name.Length > MaxNameLength)
		{
			throw new ArgumentException($"Name exceeds maximum length of {MaxNameLength} characters.");
		}
	}

	private static void GuardDescription(string? description)
	{
		if (description is not null && description.Length > MaxDescriptionLength)
		{
			throw new ArgumentException($"Description exceeds maximum length of {MaxDescriptionLength} characters.");
		}
	}
}