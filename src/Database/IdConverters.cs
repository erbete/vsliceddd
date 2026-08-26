using System;
using Domain.Authors;
using Domain.Books;
using Domain.Lending;
using Domain.Members;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Database;

internal sealed class AuthorIdConverter()
	: ValueConverter<AuthorId, Guid>(id => id.Value, value => AuthorId.From(value));

internal sealed class BookIdConverter()
	: ValueConverter<BookId, Guid>(id => id.Value, value => BookId.From(value));

internal sealed class BookItemIdConverter()
	: ValueConverter<BookItemId, Guid>(id => id.Value, value => BookItemId.From(value));

internal sealed class LendableCopyIdConverter()
	: ValueConverter<LendableCopyId, Guid>(id => id.Value, value => LendableCopyId.From(value));

internal sealed class LoanIdConverter()
	: ValueConverter<LoanId, Guid>(id => id.Value, value => LoanId.From(value));

internal sealed class MemberIdConverter()
	: ValueConverter<MemberId, Guid>(id => id.Value, value => MemberId.From(value));