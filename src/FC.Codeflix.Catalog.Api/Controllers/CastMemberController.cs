using FC.Codeflix.Catalog.Api.Contracts.Common;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.CreateCastMember;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.GetCastMember;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FC.Codeflix.Catalog.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class CastMemberController : ControllerBase
{
    private readonly IMediator _mediator;

    public CastMemberController(IMediator mediatoR)
    {
        _mediator = mediatoR;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CastMemberOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Post([FromBody] CreateCastMemberInput input, CancellationToken cancellationToken)
    {
        var output = await _mediator.Send(input, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = output.Id }, ApiResponse.Of(output));
    }

    [HttpGet("{Id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CastMemberOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid Id, CancellationToken cancellationToken)
    {
        var output = await _mediator.Send(new GetCastMemberInput(Id), cancellationToken);
        return Ok(ApiResponse.Of(output));
    }
}
