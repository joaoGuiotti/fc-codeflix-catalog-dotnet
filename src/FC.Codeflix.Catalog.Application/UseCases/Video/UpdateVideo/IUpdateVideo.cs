using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.UploadVideo;

public interface IUpdateVideo
    : IRequestHandler<UpdateVideoInput, VideoModelOutput>;
