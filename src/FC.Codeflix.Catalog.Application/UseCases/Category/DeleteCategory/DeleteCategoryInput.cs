using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Category.DeleteCategory;

public class DeleteCategoryInput : IRequest
{
    public Guid Id { get; private set; }

    public DeleteCategoryInput(Guid id)
        => Id = id;
}
