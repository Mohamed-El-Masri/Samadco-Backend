using Domain.Common;

namespace Domain.ValueObjects.Vehicles;

/// <summary>
/// كائن قيمة لمعلومات المركبة
/// </summary>
public sealed class VehicleInfo : ValueObject
{
    public string Make { get; private set; } // الماركة
    public string Model { get; private set; } // الموديل
    public int Year { get; private set; } // سنة الصنع
    public string PlateNumber { get; private set; } // رقم اللوحة
    public string Color { get; private set; } // اللون
    public string Type { get; private set; } // نوع المركبة (شاحنة، سيارة، دراجة نارية، إلخ)

    private VehicleInfo(string make, string model, int year, string plateNumber, string color, string type)
    {
        Make = make;
        Model = model;
        Year = year;
        PlateNumber = plateNumber;
        Color = color;
        Type = type;
    }

    public static VehicleInfo Create(string make, string model, int year, string plateNumber, string color, string type)
    {
        ValidateInputs(make, model, year, plateNumber, color, type);

        return new VehicleInfo(
            make.Trim(),
            model.Trim(),
            year,
            plateNumber.Trim().ToUpperInvariant(),
            color.Trim(),
            type.Trim());
    }

    private static void ValidateInputs(string make, string model, int year, string plateNumber, string color, string type)
    {
        if (string.IsNullOrWhiteSpace(make))
            throw new ArgumentException("Vehicle make is required", nameof(make));

        if (make.Length > 50)
            throw new ArgumentException("Vehicle make cannot exceed 50 characters", nameof(make));

        if (string.IsNullOrWhiteSpace(model))
            throw new ArgumentException("Vehicle model is required", nameof(model));

        if (model.Length > 50)
            throw new ArgumentException("Vehicle model cannot exceed 50 characters", nameof(model));

        var currentYear = DateTime.UtcNow.Year;
        if (year < 1900 || year > currentYear + 1)
            throw new ArgumentException($"Vehicle year must be between 1900 and {currentYear + 1}", nameof(year));

        if (string.IsNullOrWhiteSpace(plateNumber))
            throw new ArgumentException("Vehicle plate number is required", nameof(plateNumber));

        if (plateNumber.Length > 20)
            throw new ArgumentException("Vehicle plate number cannot exceed 20 characters", nameof(plateNumber));

        if (string.IsNullOrWhiteSpace(color))
            throw new ArgumentException("Vehicle color is required", nameof(color));

        if (color.Length > 30)
            throw new ArgumentException("Vehicle color cannot exceed 30 characters", nameof(color));

        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("Vehicle type is required", nameof(type));

        if (type.Length > 50)
            throw new ArgumentException("Vehicle type cannot exceed 50 characters", nameof(type));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Make;
        yield return Model;
        yield return Year;
        yield return PlateNumber;
        yield return Color;
        yield return Type;
    }

    public override string ToString() => $"{Year} {Make} {Model} - {PlateNumber}";
}
