namespace WebAPI.Features.Common;

internal abstract record ListRequest
{
	public int? Page { get; init; }
	public int? PageSize { get; init; }
}