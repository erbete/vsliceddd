using Domain.Books;
using ErrorOr;
using Shouldly;

namespace Domain.UnitTests;

public sealed class BookTests
{
    // private static Book CreateBook(string? isbn = null) => 
    //     Book.Create(Guid.NewGuid(), "Clean Code", 2008, Guid.NewGuid(), isbn);

    // [Fact]
    // public void Create_WithValidArguments_ShouldCreateBook()
    // {
    //     var id = Guid.NewGuid();
    //     var authorId = Guid.NewGuid();
    //     var book = Book.Create(id, "Clean Code", 2008, authorId, "1234567890123");

    //     book.Id.ShouldBe(id);
    //     book.Title.ShouldBe("Clean Code");
    //     book.PublishedYear.ShouldBe(2008);
    //     book.AuthorId.ShouldBe(authorId);
    //     book.Isbn.ShouldBe("1234567890123");
    //     book.BookItems.ShouldBeEmpty();
    // }

    // [Fact]
    // public void Create_ShouldTrimTitleAndIsbn()
    // {
    //     var book = Book.Create(Guid.NewGuid(), "  Clean Code  ", 2008, Guid.NewGuid(), "  123  ");
    //     book.Title.ShouldBe("Clean Code");
    //     book.Isbn.ShouldBe("123");
    // }

    // [Fact]
    // public void Create_WithEmptyId_ShouldThrowArgumentException()
    // {
    //     Should.Throw<ArgumentException>(() =>
    //         Book.Create(Guid.Empty, "Title", 2000, Guid.NewGuid())
    //     );
    // }

    // [Theory]
    // [InlineData(null)]
    // [InlineData("")]
    // [InlineData("   ")]
    // public void Create_WithInvalidTitle_ShouldThrowArgumentException(string? title)
    // {
    //     Should.Throw<ArgumentException>(() =>
    //         Book.Create(Guid.NewGuid(), title!, 2000, Guid.NewGuid())
    //     );
    // }

    // [Fact]
    // public void Create_WithTooLongTitle_ShouldThrowArgumentException()
    // {
    //     var title = new string('a', Book.MaxTitleLength + 1);
    //     Should.Throw<ArgumentException>(() =>
    //         Book.Create(Guid.NewGuid(), title, 2000, Guid.NewGuid())
    //     );
    // }

    // [Fact]
    // public void Create_WithTooLongIsbn_ShouldThrowArgumentException()
    // {
    //     var isbn = new string('a', Book.MaxIsbnLength + 1);
    //     Should.Throw<ArgumentException>(() =>
    //         Book.Create(Guid.NewGuid(), "Title", 2000, Guid.NewGuid(), isbn)
    //     );
    // }

    // [Fact]
    // public void Create_WithEmptyAuthorId_ShouldThrowArgumentException()
    // {
    //     Should.Throw<ArgumentException>(() =>
    //         Book.Create(Guid.NewGuid(), "Title", 2000, Guid.Empty)
    //     );
    // }

    // [Fact]
    // public void Create_WithFuturePublishedYear_ShouldThrowArgumentOutOfRangeException()
    // {
    //     var futureYear = DateTimeOffset.UtcNow.Year + 1;
    //     Should.Throw<ArgumentOutOfRangeException>(() =>
    //         Book.Create(Guid.NewGuid(), "Title", futureYear, Guid.NewGuid())
    //     );
    // }

    // [Fact]
    // public void Create_WithTooOldPublishedYear_ShouldThrowArgumentOutOfRangeException()
    // {
    //     Should.Throw<ArgumentOutOfRangeException>(() =>
    //         Book.Create(Guid.NewGuid(), "Title", Book.MinPublishedYear - 1, Guid.NewGuid())
    //     );
    // }

    // [Fact]
    // public void AddCopy_WithValidBarcode_ShouldSucceed()
    // {
    //     var book = CreateBook();
    //     var itemId = Guid.NewGuid();

    //     var result = book.AddCopy(itemId, "BC-001", DateOnly.FromDateTime(DateTime.UtcNow));

    //     result.IsError.ShouldBeFalse();
    //     result.Value.ShouldBe(itemId);
    //     book.BookItems.Count.ShouldBe(1);
    //     book.BookItems[0].Barcode.ShouldBe("bc-001");
    // }

    // [Fact]
    // public void AddCopy_ShouldNormalizeBarcode()
    // {
    //     var book = CreateBook();
    //     book.AddCopy(Guid.NewGuid(), "  ABC-123  ", DateOnly.FromDateTime(DateTime.UtcNow));

    //     book.BookItems[0].Barcode.ShouldBe("abc-123");
    // }

    // [Fact]
    // public void AddCopy_WithDuplicateId_ShouldReturnConflict()
    // {
    //     var book = CreateBook();
    //     var id = Guid.NewGuid();
    //     book.AddCopy(id, "BC-001", DateOnly.FromDateTime(DateTime.UtcNow));

    //     var result = book.AddCopy(id, "BC-002", DateOnly.FromDateTime(DateTime.UtcNow));

    //     result.IsError.ShouldBeTrue();
    //     result.FirstError.Type.ShouldBe(ErrorType.Conflict);
    //     result.FirstError.Code.ShouldBe("Book.DuplicateBookItemId");
    // }

    // [Fact]
    // public void AddCopy_WithDuplicateBarcode_ShouldReturnConflict()
    // {
    //     var book = CreateBook();
    //     book.AddCopy(Guid.NewGuid(), "BC-001", DateOnly.FromDateTime(DateTime.UtcNow));

    //     var result = book.AddCopy(Guid.NewGuid(), "BC-001", DateOnly.FromDateTime(DateTime.UtcNow));

    //     result.IsError.ShouldBeTrue();
    //     result.FirstError.Type.ShouldBe(ErrorType.Conflict);
    //     result.FirstError.Code.ShouldBe("Book.DuplicateBarcode");
    // }

    // [Fact]
    // public void AddCopy_WithDuplicateBarcode_CaseInsensitive_ShouldReturnConflict()
    // {
    //     var book = CreateBook();
    //     book.AddCopy(Guid.NewGuid(), "BC-001", DateOnly.FromDateTime(DateTime.UtcNow));

    //     var result = book.AddCopy(Guid.NewGuid(), "bc-001", DateOnly.FromDateTime(DateTime.UtcNow));

    //     result.IsError.ShouldBeTrue();
    //     result.FirstError.Code.ShouldBe("Book.DuplicateBarcode");
    // }

    // [Fact]
    // public void AddCopy_WithFutureAcquired_ShouldThrowArgumentOutOfRangeException()
    // {
    //     var book = CreateBook();
    //     var future = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
    //     Should.Throw<ArgumentOutOfRangeException>(() =>
    //         book.AddCopy(Guid.NewGuid(), "BC-001", future)
    //     );
    // }

    // [Fact]
    // public void UpdateDetails_WithValidArguments_ShouldUpdate()
    // {
    //     var book = CreateBook();
    //     book.UpdateDetails("New Title", 2010, "999");

    //     book.Title.ShouldBe("New Title");
    //     book.PublishedYear.ShouldBe(2010);
    //     book.Isbn.ShouldBe("999");
    // }

    // [Fact]
    // public void UpdateBarcode_WithExistingItem_ShouldUpdate()
    // {
    //     var book = CreateBook();
    //     var id = Guid.NewGuid();
    //     book.AddCopy(id, "BC-001", DateOnly.FromDateTime(DateTime.UtcNow));

    //     var result = book.UpdateBarcode(id, "BC-999");

    //     result.IsError.ShouldBeFalse();
    //     book.BookItems[0].Barcode.ShouldBe("bc-999");
    // }

    // [Fact]
    // public void UpdateBarcode_WithDuplicateBarcode_ShouldReturnConflict()
    // {
    //     var book = CreateBook();
    //     var id1 = Guid.NewGuid();
    //     var id2 = Guid.NewGuid();
    //     book.AddCopy(id1, "BC-001", DateOnly.FromDateTime(DateTime.UtcNow));
    //     book.AddCopy(id2, "BC-002", DateOnly.FromDateTime(DateTime.UtcNow));

    //     var result = book.UpdateBarcode(id2, "BC-001");

    //     result.IsError.ShouldBeTrue();
    //     result.FirstError.Code.ShouldBe("Book.DuplicateBarcode");
    // }

    // [Fact]
    // public void UpdateBarcode_WithUnknownId_ShouldReturnNotFound()
    // {
    //     var book = CreateBook();
    //     book.AddCopy(Guid.NewGuid(), "BC-001", DateOnly.FromDateTime(DateTime.UtcNow));

    //     var result = book.UpdateBarcode(Guid.NewGuid(), "BC-999");

    //     result.IsError.ShouldBeTrue();
    //     result.FirstError.Type.ShouldBe(ErrorType.NotFound);
    //     result.FirstError.Code.ShouldBe("Book.ItemNotFound");
    // }
}
