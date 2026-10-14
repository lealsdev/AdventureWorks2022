using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessEntity.Application.Common.Exceptions
{
    public sealed class ConflictException(string message) : Exception(message);

}
