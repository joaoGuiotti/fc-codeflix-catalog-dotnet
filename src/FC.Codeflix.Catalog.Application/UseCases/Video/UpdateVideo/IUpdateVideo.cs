using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Video.UploadVideo;

internal interface IUpdateVideo
    : IRequestHandler<UpdateVideoInput, VideoModelOutput>;
