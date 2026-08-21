using System.Collections.Generic;

namespace WebAPI.Features.Common;

internal sealed record PagedResponse<T>(IReadOnlyList<T> Data, PaginationMetadata Pagination);