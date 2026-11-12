using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using Microsoft.AspNetCore.Mvc;

namespace FC.Codeflix.Catalog.Api.Models.Common;

public sealed record ListRequest
{
    [FromQuery(Name = "page")] public int Page { get; init; } = 1;
    [FromQuery(Name = "per_page")] public int PerPage { get; init; } = 15;
    [FromQuery(Name = "search")] public string? Search { get; init; } = "";
    [FromQuery(Name = "sort")] public string? Sort { get; init; } = "";
    [FromQuery(Name = "sort_dir")] public ESearchOrder SortDir { get; init; } = ESearchOrder.Asc;
}
