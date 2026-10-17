using MediatR;

namespace BusinessEntity.Application.Features.BusinessEntity.Create;

public sealed record CreateBusinessEntityCommand : IRequest<CreateBusinessEntityCommandResult>;
