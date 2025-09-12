using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Domain.Repository;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;

public class CreateCategory : ICreateCategory
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICategoryRepository _categoryReposioty;

    public CreateCategory(
        ICategoryRepository categoryReposioty,
        IUnitOfWork uow
    )
    {
        _unitOfWork = uow;
        _categoryReposioty = categoryReposioty;
    }

    public async Task<CategoryModelOutput> Handle(
        CreateCategoryInput input,
        CancellationToken token = default
    )
    {
        var category = new DomainEntity.Category(input.Name, input.Description!, input.IsActive);

        await _categoryReposioty.Insert(category, token);
        await _unitOfWork.Commit(token);

        return CategoryModelOutput.FromCategory(category);
    }
}