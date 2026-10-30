using FC.Codeflix.Catalog.Application.UseCases.CastMember.ListCastMember;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.UpdateCastMember;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using Microsoft.AspNetCore.Mvc;

namespace FC.Codeflix.Catalog.Api.Contracts.CastMembers;

public sealed record UpdateCastMemberRequest(
    string Name,
    ECastMemberType? Type
);

public sealed record ListCastMemberRequest
{
    [FromQuery(Name = "page")] public int Page { get; init; } = 1;
    [FromQuery(Name = "per_page")] public int PerPage { get; init; } = 15;
    [FromQuery(Name = "search")] public string? Search { get; init; } = "";
    [FromQuery(Name = "sort")] public string? Sort { get; init; } = "";
    [FromQuery(Name = "sort_dir")] public ESearchOrder SortDir { get; init; } = ESearchOrder.Asc;
}

public static class UpdateCastMemberMappings
{
    public static UpdateCastMemberInput ToInput(this UpdateCastMemberRequest request, Guid id)
        => new(id, request.Name, request.Type);

    public static ListCastMemberInput ToInput(this ListCastMemberRequest request)
        => new(request.Page, request.PerPage, request.Search ?? "", request.Sort ?? "", request.SortDir);
}
