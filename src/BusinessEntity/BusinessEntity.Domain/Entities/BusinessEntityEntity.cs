namespace BusinessEntity.Domain.Entities;

public sealed class BusinessEntityEntity
{
    public int BusinessEntityId { get; private set; }

    public Guid RowGuid { get; private set; }

    public DateTime ModifiedDate { get; private set; }

    private BusinessEntityEntity()
    {
    }

    public static BusinessEntityEntity Create() => new();
}
