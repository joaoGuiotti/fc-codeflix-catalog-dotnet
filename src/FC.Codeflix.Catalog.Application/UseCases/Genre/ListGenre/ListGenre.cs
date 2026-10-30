using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.ListGenre;

public class ListGenre : IListGenre
{
    private readonly IGenreRepository _repo;
    private readonly ICategoryRepository _categoryRepository;

    public ListGenre(IGenreRepository repo, ICategoryRepository categoryRepository)
    {
        _repo = repo;
        _categoryRepository = categoryRepository;
    }

    public async Task<ListGenreOutput> Handle(
        ListGenreInput input,
        CancellationToken cancellationToken
    )
    {
        var searchOutput = await _repo.Search(
            input.ToSearchInput(),
            cancellationToken
        );

        var output = ListGenreOutput.FromSearchOutput(searchOutput);

        var relatedCategoriesIds = searchOutput.Items
           .SelectMany(item => item.Categories)
           .Distinct()
           .ToList();

        if (relatedCategoriesIds.Count > 0)
        {
            IReadOnlyList<Domain.Entity.Category> categories =
                await _categoryRepository
                .GetListByIds(relatedCategoriesIds, cancellationToken);

            output.FillWithCategoryNames(categories);
        }

        return output;
    }
}
