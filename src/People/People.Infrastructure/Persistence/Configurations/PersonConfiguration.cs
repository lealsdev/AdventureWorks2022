
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using People.Domain.Entities;
using People.Domain.Enums;

namespace People.Infrastructure.Persistence.Configurations
{
    public sealed class PersonConfiguration : IEntityTypeConfiguration<PersonEntity>
    {
        public void Configure(EntityTypeBuilder<PersonEntity> builder)
        {
            builder.ToTable("Person", "Person",
                table => table.ExcludeFromMigrations());

            builder.HasKey(p => p.BusinessEntityId);

            builder.Property(p => p.BusinessEntityId)
                .HasColumnName("BusinessEntityID")                
                .ValueGeneratedNever();

            builder.Property(p => p.PersonType)
                .HasColumnName("PersonType")
                .HasColumnType("nchar(2)")
                .HasConversion(
                    value => PersonTypeToDatabase(value),
                    value => PersonTypeFromDatabase(value))
                .IsRequired();

            builder.Property(p => p.NameStyle)
                .HasColumnName("NameStyle")
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(p => p.Title)
                .HasColumnName("Title")
                .HasMaxLength(8);

            builder.Property(p => p.FirstName)
                .HasColumnName("FirstName")
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.MiddleName)
                .HasColumnName("MiddleName")
                .HasMaxLength(50);

            builder.Property(p => p.LastName)
                .HasColumnName("LastName")
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.Suffix)
                .HasColumnName("Suffix")
                .HasMaxLength(10);

            builder.Property(p => p.EmailPromotion)
                .HasColumnName("EmailPromotion")
                .IsRequired()
                .HasDefaultValue(EmailPromotionPreference.None)
                .HasConversion(
                    value => EmailPromotionPreferenceToDatabase(value),
                    value => EmailPromotionPreferenceFromDatabase(value));

            builder.Property(p => p.AdditionalContactInfo)
                .HasColumnName("AdditionalContactInfo")
                .HasColumnType("xml");

            builder.Property(p => p.Demographics)
                .HasColumnName("Demographics")
                .HasColumnType("xml");

            builder.Property(p => p.RowGuid)
                .HasColumnName("rowguid")
                .HasDefaultValueSql("newid()")
                .ValueGeneratedOnAdd();

            builder.Property(p => p.ModifiedDate)
                .HasColumnName("ModifiedDate")
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()")
                .ValueGeneratedOnAdd();
        }

        private static string PersonTypeToDatabase(PersonType personType)
        {
            return personType switch
            {
                PersonType.StoreContact => "SC",
                PersonType.IndividualCustomer => "IN",
                PersonType.SalesPerson => "SP",
                PersonType.Employee => "EM",
                PersonType.VendorContact => "VC",
                PersonType.GeneralContact => "GC",
                _ => throw new ArgumentOutOfRangeException(nameof(personType))
            };
        }

        private static PersonType PersonTypeFromDatabase(string value)
        {
            return value.Trim() switch
            {
                "SC" => PersonType.StoreContact,
                "IN" => PersonType.IndividualCustomer,
                "SP" => PersonType.SalesPerson,
                "EM" => PersonType.Employee,
                "VC" => PersonType.VendorContact,
                "GC" => PersonType.GeneralContact,
                _ => throw new ArgumentOutOfRangeException(nameof(value))
            };
        }

        private static int EmailPromotionPreferenceToDatabase(EmailPromotionPreference preference)
        {
            return preference switch
            {
                EmailPromotionPreference.None => 0,
                EmailPromotionPreference.AdventureWorksOnly => 1,
                EmailPromotionPreference.AdventureWorksAndPartners => 2,
                _ => throw new ArgumentOutOfRangeException(nameof(preference))
            };
        }

        private static EmailPromotionPreference EmailPromotionPreferenceFromDatabase(int value)
        {
            return value switch
            {
                0 => EmailPromotionPreference.None,
                1 => EmailPromotionPreference.AdventureWorksOnly,
                2 => EmailPromotionPreference.AdventureWorksAndPartners,
                _ => throw new ArgumentOutOfRangeException(nameof(value))
            };
        }


    }
}
