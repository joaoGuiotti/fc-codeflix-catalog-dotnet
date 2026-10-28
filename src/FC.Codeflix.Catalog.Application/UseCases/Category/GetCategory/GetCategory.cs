using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.Application.UseCases.Category.GetCategory;

public class GetCategory : IGetCategory
{
    private readonly ICategoryRepository _repo;

    public GetCategory(ICategoryRepository categoryRepository)
    {
        _repo = categoryRepository;
    }

    public async Task<CategoryModelOutput> Handle(
        GetCategoryInput request,
        CancellationToken cancellationToken
    )
    {
        var category = await _repo.Get(request.Id, cancellationToken);
        NotFoundException.ThrowIfNull(category, request.Id);
        return CategoryModelOutput.FromCategory(category);
    }
}
