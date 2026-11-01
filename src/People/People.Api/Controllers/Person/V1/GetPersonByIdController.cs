using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using People.Api.Configuration;
using People.Application.Features.Person.GetById;

namespace People.Api.Controllers.Person.V1
{
    [ApiController]
    [ApiVersion(1.0)]
    [Tags(ApiEndpoints.Person.Tag)]
    [Route(ApiEndpoints.Person.V1.Base)]
    public class GetPersonByIdController : ControllerBase
    {
        private readonly ISender _mediator;

        public GetPersonByIdController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpGet(ApiEndpoints.Person.V1.ById, Name = ApiEndpoints.Person.V1.GetByIdName)]
        [ProducesResponseType(typeof(GetPersonByIdQueryResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        {
            var query = new GetPersonByIdQuery(id);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }
    }
}
