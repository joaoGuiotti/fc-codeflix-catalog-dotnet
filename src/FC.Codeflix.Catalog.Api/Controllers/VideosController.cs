using FC.Codeflix.Catalog.Api.Models.Common;
using FC.Codeflix.Catalog.Api.Models.Video;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;
using FC.Codeflix.Catalog.Application.UseCases.Video.DeleteVideo;
using FC.Codeflix.Catalog.Application.UseCases.Video.GetVideo;
using FC.Codeflix.Catalog.Application.UseCases.Video.ListVideo;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FC.Codeflix.Catalog.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class VideosController : ControllerBase
{
    private readonly IMediator _mediator;
    public VideosController(IMediator mediator)
        => _mediator = mediator;

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<VideoModelOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateVideo(
        [FromBody] CreateVideoInput request,
        CancellationToken cancellationToken
    )
    {
        var output = await _mediator.Send(request, cancellationToken);
        return CreatedAtAction(nameof(CreateVideo), new { id = output.Id }, ApiResponse.Of(output));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseList<VideoModelOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseList<VideoModelOutput>), StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ListVideos(
        [FromQuery] ListRequest request,
        CancellationToken cancellationToken
    )
    {
        var input = new ListVideoInput
        {
            Page = request.Page,
            PerPage = request.PerPage,
            Search = request.Search,
            Sort = request.Sort,
            SortDir = request.SortDir,
        };
        var output = await _mediator.Send(input, cancellationToken);
        return Ok(ApiResponse.OfList(output));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<VideoModelOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        var output = await _mediator.Send(new GetVideoInput(id), cancellationToken);
        return Ok(ApiResponse.Of(output));
    }


    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseList<VideoModelOutput>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteVideo(
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        var input = new DeleteVideoInput(id);
        await _mediator.Send(input, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<VideoModelOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateVideo(
        [FromRoute] Guid id,
        [FromBody] UpdateVideoApiInput apiInput,
        CancellationToken cancellationToken)
    {
        var output = await _mediator.Send(
            apiInput.ToInput(id),
            cancellationToken
        );
        return Ok(ApiResponse.Of(output));
    }

    [HttpPost("{id:guid}/medias/{type}")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UploadMedia(
        [FromRoute] Guid id,
        [FromRoute] string type,
        [FromForm] UploadMediaApiInput apiInput,
        CancellationToken cancellationToken
    )
    {
        var input = apiInput.ToUploadMediasInput(id, type);
        await _mediator.Send(input, cancellationToken);
        return NoContent();
    }
}
