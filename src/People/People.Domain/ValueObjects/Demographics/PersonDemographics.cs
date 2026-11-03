using System.Collections.Immutable;

namespace People.Domain.ValueObjects.Demographics
{
    public sealed record PersonDemographics
    {
        public decimal? TotalPurchaseYTD { get; init; }
        public SurveyDate? DateFirstPurchase { get; init; }
        public SurveyDate? BirthDate { get; init; }
        public string? MaritalStatus { get; init; }
        public string? YearlyIncome { get; init; }
        public string? Gender { get; init; }
        public int? TotalChildren { get; init; }
        public int? NumberChildrenAtHome { get; init; }
        public string? Education { get; init; }
        public string? Occupation { get; init; }
        public string? HomeOwnerFlag { get; init; }
        public int? NumberCarsOwned { get; init; }
        public ImmutableArray<string> Hobbies { get; init; } = ImmutableArray<string>.Empty;
        public string? CommuteDistance { get; init; }
        public string? Comments { get; init; }
    }
}
