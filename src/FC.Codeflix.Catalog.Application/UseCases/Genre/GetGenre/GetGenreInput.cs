using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.GetGenre;

public sealed record GetGenreInput(Guid Id)
    : IRequest<GenreModelOutput>
{ }
