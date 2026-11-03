using System;
using System.Collections.Generic;
using System.Text;

namespace People.Domain.ValueObjects.AdditionalContactInfo
{
    public sealed record ContactEmail(
        string Address, 
        ContactInstructions? Instructions = null
    );
}
