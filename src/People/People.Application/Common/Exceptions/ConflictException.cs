using System;
using System.Collections.Generic;
using System.Text;

namespace People.Application.Common.Exceptions
{
    public sealed class ConflictException(string message) : Exception(message);

}
