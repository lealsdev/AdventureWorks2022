using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace People.Domain.ValueObjects.AdditionalContactInfo
{
    public sealed record ContactAddress(
        AddressKind Kind, 
        ImmutableArray<string> Streets, 
        string City,
        string StateProvince, 
        string? PostalCode, 
        string CountryRegion, 
        ContactInstructions? Instructions = null
    );
}
