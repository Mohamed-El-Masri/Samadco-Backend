using Domain.Common;

namespace Domain.ValueObjects.Location;

/// <summary>
/// كائن قيمة للعنوان
/// </summary>
public sealed class Address : ValueObject
{
    public string Country { get; private set; }
    public string City { get; private set; }
    public string Line1 { get; private set; }
    public string? Line2 { get; private set; }
    public string? PostalCode { get; private set; }

    private Address(string country, string city, string line1, string? line2, string? postalCode)
    {
        Country = country;
        City = city;
        Line1 = line1;
        Line2 = line2;
        PostalCode = postalCode;
    }

    public static Address Create(string country, string city, string line1, string? line2 = null, string? postalCode = null)
    {
        if (string.IsNullOrWhiteSpace(country))
            throw new ArgumentException("Country cannot be null or empty", nameof(country));

        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be null or empty", nameof(city));

        if (string.IsNullOrWhiteSpace(line1))
            throw new ArgumentException("Address line 1 cannot be null or empty", nameof(line1));

        if (country.Length > 50)
            throw new ArgumentException("Country cannot exceed 50 characters", nameof(country));

        if (city.Length > 50)
            throw new ArgumentException("City cannot exceed 50 characters", nameof(city));

        if (line1.Length > 100)
            throw new ArgumentException("Address line 1 cannot exceed 100 characters", nameof(line1));

        if (!string.IsNullOrWhiteSpace(line2) && line2.Length > 100)
            throw new ArgumentException("Address line 2 cannot exceed 100 characters", nameof(line2));

        if (!string.IsNullOrWhiteSpace(postalCode) && postalCode.Length > 20)
            throw new ArgumentException("Postal code cannot exceed 20 characters", nameof(postalCode));

        return new Address(country.Trim(), city.Trim(), line1.Trim(), 
            string.IsNullOrWhiteSpace(line2) ? null : line2.Trim(),
            string.IsNullOrWhiteSpace(postalCode) ? null : postalCode.Trim());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Country;
        yield return City;
        yield return Line1;
        yield return Line2;
        yield return PostalCode;
    }

    public override string ToString()
    {
        var parts = new List<string> { Line1 };
        
        if (!string.IsNullOrWhiteSpace(Line2))
            parts.Add(Line2);
            
        parts.Add(City);
        parts.Add(Country);
        
        if (!string.IsNullOrWhiteSpace(PostalCode))
            parts.Add(PostalCode);
            
        return string.Join(", ", parts);
    }
}
