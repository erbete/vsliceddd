namespace WebAPI.Features.Common;

internal sealed record Response<T>(T Data, ResponseMetadata Metadata)
{
    public static Response<T> Create(T data, string traceId)
        => new(data, ResponseMetadata.Create(traceId));

    public static Response<T> Create(T data, string traceId, PaginationMetadata pagination)
        => new(data, ResponseMetadata.Create(traceId, pagination));
}