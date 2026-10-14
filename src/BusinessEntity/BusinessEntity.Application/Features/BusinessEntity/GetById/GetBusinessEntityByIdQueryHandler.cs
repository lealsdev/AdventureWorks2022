using BusinessEntity.Application.Abstractions.Persistence;
using BusinessEntity.Application.Common.Exceptions;
using BusinessEntity.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessEntity.Application.Features.BusinessEntity.GetById
{
    public class GetBusinessEntityByIdQueryHandler : IRequestHandler<GetBusinessEntityByIdQuery, GetBusinessEntityByIdResult>
    {
        private readonly IBusinessEntityRepository _repository;

        public GetBusinessEntityByIdQueryHandler(IBusinessEntityRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetBusinessEntityByIdResult> Handle(GetBusinessEntityByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (entity == null)
            {
                throw new NotFoundException(nameof(BusinessEntityEntity));
            }

            return new GetBusinessEntityByIdResult
            (
                BusinessEntityId: entity.BusinessEntityId,
                RowGuid: entity.RowGuid,
                ModifiedDate: entity.ModifiedDate
            );
        }
    }
}
