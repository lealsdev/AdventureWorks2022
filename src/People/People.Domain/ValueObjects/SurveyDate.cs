namespace People.Domain.ValueObjects
{
    public sealed record SurveyDate(
        DateOnly Value,
        int? OffsetMinutes = null
    );
}
