using Domain.Common;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects.Identity;

/// <summary>
/// كائن قيمة لرقم الهاتف
/// </summary>
public sealed class PhoneNumber : ValueObject
{
    private static readonly Regex PhoneRegex = new(
        @"^\+[1-9]\d{1,14}$", // E.164 format
        RegexOptions.Compiled);

    public string Value { get; private set; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static PhoneNumber Create(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be null or empty", nameof(phoneNumber));

        // إزالة المسافات والشرطات والأقواس
        var cleanedPhone = phoneNumber.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");

        // إضافة + إذا لم تكن موجودة
        if (!cleanedPhone.StartsWith("+"))
        {
            // افتراض أن الرقم يبدأ برمز المملكة العربية السعودية +966
            if (cleanedPhone.StartsWith("966"))
                cleanedPhone = "+" + cleanedPhone;
            else if (cleanedPhone.StartsWith("0"))
                cleanedPhone = "+966" + cleanedPhone[1..];
            else
                cleanedPhone = "+966" + cleanedPhone;
        }

        if (!PhoneRegex.IsMatch(cleanedPhone))
            throw new ArgumentException("Invalid phone number format. Use E.164 format (+966xxxxxxxxx)", nameof(phoneNumber));

        return new PhoneNumber(cleanedPhone);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(PhoneNumber phoneNumber) => phoneNumber.Value;
}
