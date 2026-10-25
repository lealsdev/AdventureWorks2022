using People.Domain.Enums;

namespace People.Domain.Entities
{
    public sealed class PersonEntity
    {
        public int BusinessEntityId { get; private set; }

        public PersonType PersonType { get; private set; }

        public bool NameStyle { get; private set; }

        public string? Title { get; private set; }

        public string FirstName { get; private set; } = null!;

        public string? MiddleName { get; private set; }

        public string LastName { get; private set; } = null!;

        public string? Suffix { get; private set; }

        public int EmailPromotion { get; private set; }

        public string? AdditionalContactInfo { get; private set; }

        public string? Demographics { get; private set; }

        public Guid RowGuid { get; private set; }

        public DateTime ModifiedDate { get; private set; }

        private PersonEntity()
        {
            
        }
    }
}
