using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.GetGenre;

public class GetGenre : IGetGenre
{
    private readonly IGenreRepository _repo;
    private readonly ICategoryRepository _categoryRepository;

    public GetGenre(IGenreRepository repo, ICategoryRepository categoryRepository)
        => (_repo, _categoryRepository) = (repo, categoryRepository);

    public async Task<GenreModelOutput> Handle(GetGenreInput request, CancellationToken cancellationToken)
    {
        var genre = await _repo.Get(request.Id, cancellationToken);
        NotFoundException.ThrowIfNull(genre, request.Id);
        var output = GenreModelOutput.FromEntity(genre);

        if (output.Categories.Count > 0)
        {
            var categories = (await _categoryRepository
                .GetListByIds(
                    output.Categories.Select(x => x.Id).ToList(), cancellationToken)
                ).ToDictionary(x => x.Id);
            foreach (var category in output.Categories)
                category.Name = categories[category.Id].Name;
        }

        return output;
    }
}
