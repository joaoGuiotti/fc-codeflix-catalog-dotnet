using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Genre.DeleteGenre;

public sealed record DeleteGenreInput(Guid Id) : IRequest; 
