using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.ListVideo;

public class ListVideo : IListVideo
{

    public readonly IVideoRepository _repo;
    private readonly ICategoryRepository _categoryRepo;
    private readonly IGenreRepository _genreRepo;
    private readonly ICastMemberRepository _castMemberRepo;

    public ListVideo(
        IVideoRepository repo,
        ICategoryRepository categoryRepo,
        IGenreRepository genreRepo,
        ICastMemberRepository castMemberRepo
    )
    {
        _repo = repo;
        _categoryRepo = categoryRepo;
        _genreRepo = genreRepo;
        _castMemberRepo = castMemberRepo;
    }

    public async Task<ListVideoOutput> Handle(ListVideoInput request, CancellationToken ct)
    {

        var searchOutput = await _repo.Search(request.ToSearchInput(), ct);

        var categories = await GetCategoriesReleatedsAsync(searchOutput.Items, ct);
        var genres = await GetGenresReleatedsAsync(searchOutput.Items, ct);
        var castMembers = await GetCastMembersReleatedsAsync(searchOutput.Items, ct);

        return ListVideoOutput.FromSearchOutput(searchOutput, categories, genres, castMembers);
    }

    private async Task<IReadOnlyList<Domain.Entity.Genre>> GetGenresReleatedsAsync(
        IReadOnlyList<Domain.Entity.Video> videos,
        CancellationToken ct
    )
    {
        var releatedGenresIds = videos
            .SelectMany(item => item.Genres)
            .Distinct()
            .ToList();

        if (releatedGenresIds.Count == 0)
            return Array.Empty<Domain.Entity.Genre>();

        return await _genreRepo.GetListByIds(releatedGenresIds, ct);
    }

    private async Task<IReadOnlyList<Domain.Entity.Category>> GetCategoriesReleatedsAsync(
        IReadOnlyList<Domain.Entity.Video> videos,
        CancellationToken ct
    )
    {
        var relatedCategoriesIds = videos
           .SelectMany(item => item.Categories)
           .Distinct()
           .ToList();

        if (relatedCategoriesIds.Count == 0)
            return Array.Empty<Domain.Entity.Category>();

        return await _categoryRepo.GetListByIds(relatedCategoriesIds, ct);
    }

    private async Task<IReadOnlyList<Domain.Entity.CastMember>> GetCastMembersReleatedsAsync(
       IReadOnlyList<Domain.Entity.Video> videos,
       CancellationToken ct
   )
    {
        var relatedCastMembersIds = videos
           .SelectMany(item => item.CastMembers)
           .Distinct()
           .ToList();

        if (relatedCastMembersIds.Count == 0)
            return Array.Empty<Domain.Entity.CastMember>();

        return await _castMemberRepo.GetListByIds(relatedCastMembersIds, ct);
    }

}
