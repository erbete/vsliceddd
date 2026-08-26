using ErrorOr;

namespace Domain.Authors;

public static class AuthorErrors
{
    public static Error NotFound(AuthorId id) =>
        Error.NotFound("Author.AuthorNotFound", $"Author with ID {id} not found.");
}