using System;

namespace WebAPI.Features.Common;

internal sealed record ResponseMetadata(DateTimeOffset Timestamp, string TraceId, PaginationMetadata? Pagination = null)
{
		public static ResponseMetadata Create(string traceId, PaginationMetadata? pagination = null)
			=> new(DateTimeOffset.UtcNow, traceId, pagination);
}

internal sealed record PaginationMetadata(int Page, int PageSize, int TotalCount, int TotalPages, PaginationLinks Links);