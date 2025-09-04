using Domain.Common;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects.Identity;

/// <summary>
/// كائن قيمة للرقم الضريبي
/// </summary>
public sealed class TaxId : ValueObject
{
    private static readonly Regex TaxIdRegex = new(
        @"^\d{15}$", // الرقم الضريبي في السعودية 15 رقم
        RegexOptions.Compiled);

    public string Value { get; private set; }

    private TaxId(string value)
    {
        Value = value;
    }

    public static TaxId Create(string taxId)
    {
        if (string.IsNullOrWhiteSpace(taxId))
            throw new ArgumentException("Tax ID cannot be null or empty", nameof(taxId));

        var cleanedTaxId = taxId.Replace(" ", "").Replace("-", "");

        if (!TaxIdRegex.IsMatch(cleanedTaxId))
            throw new ArgumentException("Invalid Tax ID format. Must be 15 digits", nameof(taxId));

        return new TaxId(cleanedTaxId);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(TaxId taxId) => taxId.Value;
}
