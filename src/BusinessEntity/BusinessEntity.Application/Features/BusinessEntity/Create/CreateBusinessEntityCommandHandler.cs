using BusinessEntity.Application.Abstractions.Persistence;
using BusinessEntity.Domain.Entities;
using MediatR;

namespace BusinessEntity.Application.Features.BusinessEntity.Create;

public sealed class CreateBusinessEntityCommandHandler(
    IBusinessEntityRepository repository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateBusinessEntityCommand, CreateBusinessEntityCommandResult>
{
    public async Task<CreateBusinessEntityCommandResult> Handle(
        CreateBusinessEntityCommand request,
        CancellationToken cancellationToken)
    {
        var entity = BusinessEntityEntity.Create();

        await repository.AddAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateBusinessEntityCommandResult(entity.BusinessEntityId);
    }
}
