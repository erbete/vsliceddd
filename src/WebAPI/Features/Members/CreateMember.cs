using System;
using System.Threading;
using System.Threading.Tasks;
using Database;
using Domain.Members;
using EntityFramework.Exceptions.Common;
using ErrorOr;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using WebAPI.Infrastructure.Errors;

namespace WebAPI.Features.Members;

internal static class CreateMember
{
    internal sealed record Request(string Name, string Email, DateOnly MembershipDate);
    internal sealed record Response(Guid Id, string Name, string Email, DateOnly MembershipDate);

    internal sealed class Validator : AbstractValidator<Request>
    {
        public Validator(TimeProvider timeProvider)
        {
            var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);

            RuleFor(r => r.Name)
                .NotEmpty()
                .MaximumLength(Member.MaxNameLength);

            RuleFor(r => r.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(Member.MaxEmailLength);

            RuleFor(r => r.MembershipDate)
                .LessThanOrEqualTo(today)
                .WithMessage("Membership date cannot be in the future.");
        }
    }

    internal sealed class Handler(AppDbContext db)
    {
        public async Task<ErrorOr<Response>> HandleAsync(Request request, CancellationToken ct)
        {
            var member = Member.Create(request.Name, request.Email, request.MembershipDate);
            db.Members.Add(member);

            try
            {
                await db.SaveChangesAsync(ct);
            }
            catch (UniqueConstraintException)
            {
                return MemberErrors.DuplicateEmail(request.Email);
            }

            return new Response(member.Id.Value, member.Name, member.Email, member.MembershipDate);
        }
    }

    internal static async Task<Results<CreatedAtRoute<Response>, ProblemHttpResult>> Endpoint(
        Request request,
        Handler handler,
        CancellationToken ct)
    {
        var result = await handler.HandleAsync(request, ct);

        return result.IsError
            ? Problems.From(result.FirstError)
            : TypedResults.CreatedAtRoute(result.Value, nameof(GetMemberById), new { id = result.Value.Id });
    }
}