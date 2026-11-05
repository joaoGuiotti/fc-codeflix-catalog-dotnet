using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.GetVideo;

public class GetVideo : IGetVideo
{
    private readonly IVideoRepository _repository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IGenreRepository _genreRepository;

    public GetVideo(IVideoRepository repository, ICategoryRepository categoryRepository, IGenreRepository genreRepository)
    {
        _repository = repository;
        _categoryRepository = categoryRepository;
        _genreRepository = genreRepository;
    }

    public async Task<VideoModelOutput> Handle(
        GetVideoInput request,
        CancellationToken cancellationToken
    )
    {
        var video = await _repository.Get(request.Id, cancellationToken);

        IReadOnlyList<Domain.Entity.Category>? categories = null;
        if (video.Categories.Count > 0)
            categories = await _categoryRepository.GetListByIds(
                video.Categories.Distinct().ToList(),
                cancellationToken
            );

        IReadOnlyList<Domain.Entity.Genre>? genres = null;
        if (video.Genres.Count > 0)
            genres = await _genreRepository.GetListByIds(
                video.Genres.Distinct().ToList(),
                cancellationToken
            );

        return VideoModelOutput.FromEntity(video, categories, genres);
    }
}
