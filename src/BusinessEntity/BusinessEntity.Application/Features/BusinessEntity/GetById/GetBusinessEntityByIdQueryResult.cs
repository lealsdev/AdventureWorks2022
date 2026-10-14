using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessEntity.Application.Features.BusinessEntity.GetById
{
    public sealed record GetBusinessEntityByIdResult(
        int BusinessEntityId,
        Guid RowGuid,
        DateTime ModifiedDate
    );
}
