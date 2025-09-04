using Domain.ValueObjects.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Converters;

/// <summary>
/// محول لـ Email Value Object
/// </summary>
public class EmailConverter : ValueConverter<Email, string>
{
    public EmailConverter()
        : base(
            email => email.Value,
            value => Email.Create(value))
    {
    }
}

/// <summary>
/// محول لـ PhoneNumber Value Object
/// </summary>
public class PhoneNumberConverter : ValueConverter<PhoneNumber, string>
{
    public PhoneNumberConverter()
        : base(
            phone => phone.Value,
            value => PhoneNumber.Create(value))
    {
    }
}

/// <summary>
/// محول لـ TaxId Value Object
/// </summary>
public class TaxIdConverter : ValueConverter<TaxId, string>
{
    public TaxIdConverter()
        : base(
            taxId => taxId.Value,
            value => TaxId.Create(value))
    {
    }
}
