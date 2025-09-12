using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;

public class CreateCategoryInput(
    string name,
    string? description,
    bool isActive = true
) : IRequest<CategoryModelOutput>
{
    public string Name { get; set; } = name;
    public string? Description { get; set; } = description ?? string.Empty;

    public bool IsActive { get; set; } = isActive;
}