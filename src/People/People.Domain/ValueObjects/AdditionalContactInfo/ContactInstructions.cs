using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace People.Domain.ValueObjects.AdditionalContactInfo
{
    public sealed record ContactInstructions(
        ImmutableArray<ContactContent> Content
    );
}
