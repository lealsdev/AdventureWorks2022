using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessEntity.Infrastructure.Persistence.Configurations
{
    public sealed class BusinessEntityConfiguration
    : IEntityTypeConfiguration<BusinessEntity.Domain.Entities.BusinessEntityEntity>
    {
        public void Configure(
            EntityTypeBuilder<BusinessEntity.Domain.Entities.BusinessEntityEntity> builder)
        {
            builder.ToTable(
                "BusinessEntity",
                "Person",
                table => table.ExcludeFromMigrations());

            builder.HasKey(x => x.BusinessEntityId);

            builder.Property(x => x.BusinessEntityId)
                .HasColumnName("BusinessEntityID")
                .UseIdentityColumn();

            builder.Property(x => x.RowGuid)
                .HasColumnName("rowguid")
                .HasDefaultValueSql("newid()")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.ModifiedDate)
                .HasDefaultValueSql("getdate()")
                .ValueGeneratedOnAdd();
        }
    }
}
