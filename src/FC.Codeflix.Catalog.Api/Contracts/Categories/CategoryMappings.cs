using FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;

namespace FC.Codeflix.Catalog.Api.Contracts.Categories;

public static class CategoryMappings
{
    public static UpdateCategoryInput ToInput(this UpdateCategoryRequest request, Guid id) =>
        new(
            id,
            request.Name,
            request.Description,
            request.IsActive
        );

    public static ListCategoriesInput ToInput(this ListCategoriesRequest request) =>
        new(request.Page, request.PerPage, request.Search, request.Sort, request.SortDir);
}
