using FC.Codeflix.Catalog.Api.Contracts.CastMembers;
using FC.Codeflix.Catalog.Api.Contracts.Common;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.CreateCastMember;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.DeleteCastMember;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.GetCastMember;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FC.Codeflix.Catalog.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class CastMemberController(IMediator _mediatoR) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CastMemberOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Post([FromBody] CreateCastMemberInput input, CancellationToken cancellationToken)
    {
        var output = await _mediatoR.Send(input, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = output.Id }, ApiResponse.Of(output));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CastMemberOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var output = await _mediatoR.Send(new GetCastMemberInput(id), cancellationToken);
        return Ok(ApiResponse.Of(output));
    }

    [HttpPut("{Id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CastMemberOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Put(
        [FromRoute] Guid id,
        [FromBody] UpdateCastMemberRequest request,
        CancellationToken cancellationToken
    )
    {
        var output = await _mediatoR.Send(request.ToInput(id), cancellationToken);
        return Ok(ApiResponse.Of(output));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CastMemberOutput>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var input = new DeleteCastMemberInput(id);
        await _mediatoR.Send(input, cancellationToken);
        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] ListCastMemberRequest request,
        CancellationToken cancellationToken
    )
    {
        var output = await _mediatoR.Send(request.ToInput(), cancellationToken);
        return Ok(ApiResponse.OfList(output));
    }
}
