using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.ListVideo;

public interface IListVideo
    : IRequestHandler<ListVideoInput, ListVideoOutput>;
