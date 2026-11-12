using FC.Codeflix.Catalog.Api.Models.Common;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Application.UseCases.Video.CreateVideo;
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
}
