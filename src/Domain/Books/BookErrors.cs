using Domain.Authors;
using ErrorOr;

namespace Domain.Books;

public static class BookErrors
{
    public static Error AuthorNotFound(AuthorId authorId) =>
        Error.NotFound("Book.AuthorNotFound", $"Author with ID {authorId} not found.");

    public static Error BookItemNotFound(BookItemId bookItemId) =>
        Error.NotFound("Book.ItemNotFound", $"Book item with id '{bookItemId}' was not found.");

    public static Error DuplicateIsbn(string title, string? isbn) =>
        Error.Conflict("Book.DuplicateIsbn", $"Book '{title}' already exists with ISBN '{isbn}'.");

    public static Error DuplicateBarcode(string barcode) =>
        Error.Conflict("Book.DuplicateBarcode", $"A copy with barcode '{barcode}' already exists for this book.");

    public static Error NotFound(BookId id) =>
        Error.NotFound("Book.NotFound", $"Book with ID {id} not found.");
}