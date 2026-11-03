using System;
using System.Collections.Generic;
using System.Text;

namespace People.Domain.ValueObjects.AdditionalContactInfo
{
    public sealed record ContactPhone(
        PhoneKind Kind, 
        string Number, 
        ContactInstructions? Instructions = null
    );
}
