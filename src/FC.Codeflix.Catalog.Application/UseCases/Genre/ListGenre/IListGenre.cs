using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.ListGenre;

public interface IListGenre
    : IRequestHandler<ListGenreInput, ListGenreOutput>
{
}
