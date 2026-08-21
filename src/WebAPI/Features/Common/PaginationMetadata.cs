namespace WebAPI.Features.Common;

internal sealed record PaginationMetadata(int Page, int PageSize, int TotalCount, int TotalPages, PaginationLinks Links);