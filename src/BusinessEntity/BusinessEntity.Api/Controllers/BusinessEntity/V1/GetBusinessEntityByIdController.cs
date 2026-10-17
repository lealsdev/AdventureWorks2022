using Asp.Versioning;
using BusinessEntity.Api.Configuration;
using BusinessEntity.Application.Features.BusinessEntity.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BusinessEntity.Api.Features.BusinessEntity.V1.GetById;

[ApiController]
[ApiVersion(1.0)]
[Route(ApiEndpoints.BusinessEntity.V1.Base)]
public sealed class GetBusinessEntityByIdController : ControllerBase
{
    private readonly ISender _mediator;

    public GetBusinessEntityByIdController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet(ApiEndpoints.BusinessEntity.V1.ById, Name = ApiEndpoints.BusinessEntity.V1.GetByIdName)]
    [ProducesResponseType(typeof(GetBusinessEntityByIdResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
    {
        var query = new GetBusinessEntityByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }
}