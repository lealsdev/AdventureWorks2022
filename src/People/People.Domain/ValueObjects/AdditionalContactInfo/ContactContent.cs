using System;
using System.Collections.Generic;
using System.Text;

namespace People.Domain.ValueObjects.AdditionalContactInfo
{
    public sealed record ContactContent
    {
        public string? Text { get; }
        public ContactPhone? Phone { get; }
        public ContactEmail? Email { get; }
        public ContactAddress? Address { get; }
        public ContactRecord? Record { get; }

        public ContactContent(
            string? text = null,
            ContactPhone? phone = null,
            ContactEmail? email = null,
            ContactAddress? address = null,
            ContactRecord? record = null)
        {
            var populatedCount =
                (text is not null ? 1 : 0) +
                (phone is not null ? 1 : 0) +
                (email is not null ? 1 : 0) +
                (address is not null ? 1 : 0) +
                (record is not null ? 1 : 0);

            if (populatedCount != 1)
            {
                throw new ArgumentException(
                    "Exactly one contact content value must be provided.");
            }

            Text = text;
            Phone = phone;
            Email = email;
            Address = address;
            Record = record;
        }
    }
}
