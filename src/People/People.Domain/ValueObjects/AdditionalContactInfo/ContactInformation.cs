using System.Collections.Immutable;
using People.Domain.ValueObjects;
namespace People.Domain.ValueObjects.AdditionalContactInfo;

public sealed record ContactInformation(
    ImmutableArray<ContactContent> Content
);