using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.UpdateGenre;

public sealed record UpdateGenreInput(
    Guid Id,
    string Name,
    bool? isActive = null,
    List<Guid>? CategoriesIds = null
) : IRequest<GenreModelOutput>;
