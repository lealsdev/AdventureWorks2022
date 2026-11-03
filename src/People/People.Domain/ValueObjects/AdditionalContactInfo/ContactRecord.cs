using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace People.Domain.ValueObjects.AdditionalContactInfo
{
    public sealed record ContactRecord(
        ImmutableArray<ContactContent> Content, 
        SurveyDate? Date = null
    );
}
