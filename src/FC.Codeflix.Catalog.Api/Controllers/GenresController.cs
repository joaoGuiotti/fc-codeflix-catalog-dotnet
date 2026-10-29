using FC.Codeflix.Catalog.Api.Contracts.Common;
using FC.Codeflix.Catalog.Application.UseCases.Genre.Common;
using FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;
using FC.Codeflix.Catalog.Application.UseCases.Genre.DeleteGenre;
using FC.Codeflix.Catalog.Application.UseCases.Genre.GetGenre;
using FC.Codeflix.Catalog.Application.UseCases.Genre.ListGenre;
using FC.Codeflix.Catalog.Application.UseCases.Genre.UpdateGenre;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FC.Codeflix.Catalog.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class GenresController : ControllerBase
{
    private readonly IMediator _mediatR;

    public GenresController(IMediator mediatR)
    {
        _mediatR = mediatR;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseList<GenreModelOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponseList<GenreModelOutput>), StatusCodes.Status204NoContent)]
    public async Task<IActionResult> List([FromQuery] ListRequest request, CancellationToken cancellationToken)
    {
        var input = new ListGenreInput(
            request.Page,
            request.PerPage,
            request.Search,
            request.Sort,
            request.sortDir
        );
        var output = await _mediatR.Send(input, cancellationToken);
        return Ok(ApiResponse.OfList(output));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(
        typeof(ApiResponse<GenreModelOutput>),
        StatusCodes.Status200OK
    )]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var input = new GetGenreInput(id);
        var output = await _mediatR.Send(input, cancellationToken);
        return Ok(ApiResponse.Of(output));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<GenreModelOutput>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Post(
        [FromBody] CreateGenreInput input,
        CancellationToken cancellationToken
    )
    {
        var output = await _mediatR.Send(input, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = output.Id }, ApiResponse.Of(output));
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<GenreModelOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Put(Guid id, [FromBody] UpdateGenreInput request, CancellationToken cancellationToken)
    {
        var input = new UpdateGenreInput(id, request.Name, request.isActive, request.CategoriesIds);
        var output = await _mediatR.Send(input, cancellationToken);
        return Ok(ApiResponse.Of(output));
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await _mediatR.Send(new DeleteGenreInput(id), cancellationToken);
        return Ok();
    }
}
