using Asp.Versioning;
using BusinessEntity.Api.Configuration;
using BusinessEntity.Api.Features.BusinessEntity.V1.GetById;
using BusinessEntity.Application.Features.BusinessEntity.Create;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BusinessEntity.Api.Controllers.BusinessEntity.V1;

[ApiController]
[ApiVersion(1.0)]
[Route(ApiEndpoints.BusinessEntity.V1.Base)]
public sealed class CreateBusinessEntityController(ISender mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(CreateBusinessEntityCommandResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateBusinessEntityCommand(), cancellationToken);

        return CreatedAtRoute(ApiEndpoints.BusinessEntity.V1.GetByIdName, new { id = result.BusinessEntityId }, result);
    }
}
