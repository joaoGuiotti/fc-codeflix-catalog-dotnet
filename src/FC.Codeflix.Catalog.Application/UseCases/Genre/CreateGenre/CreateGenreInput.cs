using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;

public sealed record CreateGenreInput(
    string Name,
    bool IsActive = true,
    List<Guid>? CategoriesIds = null
) : IRequest<GenreModelOutput>;

