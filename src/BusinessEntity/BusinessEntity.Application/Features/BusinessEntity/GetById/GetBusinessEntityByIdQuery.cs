using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessEntity.Application.Features.BusinessEntity.GetById
{
    public class GetBusinessEntityByIdQuery : IRequest<GetBusinessEntityByIdResult>
    {
        public int Id { get; }

        public GetBusinessEntityByIdQuery(int id)
        {
            Id = id;
        }
    }
}
