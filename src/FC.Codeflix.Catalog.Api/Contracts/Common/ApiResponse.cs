using FC.Codeflix.Catalog.Application.Common;

namespace FC.Codeflix.Catalog.Api.Contracts.Common;

public sealed record ApiResponse<TData>(TData Data)
    where TData : class;

public sealed record ApiResponseList<TData>(
    IReadOnlyList<TData> Data,
    ApiResponseMeta Meta
) where TData : class;

public sealed record ApiResponseMeta(int CurrentPage, int PerPage, int Total);

public static class ApiResponse
{
    public static ApiResponse<T> Of<T>(T data)
        where T : class => new(data);

    public static ApiResponseList<T> OfList<T>(PaginatedListOutput<T> output)
        where T : class =>
        new(output.Items, new ApiResponseMeta(output.Page, output.PerPage, output.Total));
}



