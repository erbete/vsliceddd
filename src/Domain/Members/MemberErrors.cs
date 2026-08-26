using ErrorOr;

namespace Domain.Members;

public static class MemberErrors
{
    public static Error NotFound(MemberId id) =>
        Error.NotFound("Member.NotFound", $"Member with ID {id} was not found.");

    public static Error DuplicateEmail(string email) =>
        Error.Conflict("Member.DuplicateEmail", $"A member with email '{email}' already exists.");
}