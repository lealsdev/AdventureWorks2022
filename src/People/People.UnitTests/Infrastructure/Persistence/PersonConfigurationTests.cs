using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using People.Domain.Entities;
using People.Domain.ValueObjects.AdditionalContactInfo;
using People.Domain.ValueObjects.Demographics;
using People.Infrastructure.Persistence;

namespace People.UnitTests.Infrastructure.Persistence;

public sealed class PersonConfigurationTests
{
    [Fact]
    public void Metadata_PreservesExistingTableAndTypedXmlConversions()
    {
        using var context = new PersonDbContext(new DbContextOptionsBuilder<PersonDbContext>()
            .UseSqlServer("Server=unused;Database=unused;Integrated Security=true;TrustServerCertificate=true").Options);

        var model = context.GetService<IDesignTimeModel>().Model;
        var entity = model.FindEntityType(typeof(PersonEntity))!;

        Assert.Equal("Person", entity.GetTableName());
        Assert.Equal("Person", entity.GetSchema());
        Assert.True(entity.IsTableExcludedFromMigrations());
        Assert.Equal(ValueGenerated.Never, entity.FindProperty(nameof(PersonEntity.BusinessEntityId))!.ValueGenerated);
        
        Check(entity.FindProperty(nameof(PersonEntity.AdditionalContactInfo))!,
            new ContactInformation([new(phone: new(PhoneKind.Mobile, "123"))]),
            new ContactInformation([new(phone: new(PhoneKind.Mobile, "456"))]));
        
        Check(entity.FindProperty(nameof(PersonEntity.Demographics))!,
            new PersonDemographics { Hobbies = ["cycling"] }, new PersonDemographics { Hobbies = ["music"] });
    }

    private static void Check(IProperty property, object value, object different)
    {
        Assert.Equal("xml", property.GetColumnType());
        Assert.Equal(property.Name, property.GetColumnName());

        var converter = property.GetValueConverter()!;
        var comparer = property.GetValueComparer()!;
        var xml = Assert.IsType<string>(converter.ConvertToProvider(value));
        var clone = converter.ConvertFromProvider(xml)!;

        Assert.True(comparer.Equals(value, clone));
        Assert.Equal(comparer.GetHashCode(value), comparer.GetHashCode(clone));
        Assert.False(comparer.Equals(value, different));
        Assert.Same(value, comparer.Snapshot(value));
        Assert.Null(converter.ConvertFromProvider(null));
        Assert.Null(converter.ConvertToProvider(null));
    }
}
