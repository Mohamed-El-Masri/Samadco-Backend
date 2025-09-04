using Domain.Common;

namespace Domain.ValueObjects.Commerce;

/// <summary>
/// كائن قيمة للقطة السلة (غير قابلة للتغيير)
/// </summary>
public sealed class CartSnapshot : ValueObject
{
    public string JsonData { get; private set; }
    public int ItemsCount { get; private set; }
    public DateTime SnapshotTakenAtUtc { get; private set; }

    private CartSnapshot(string jsonData, int itemsCount, DateTime snapshotTakenAtUtc)
    {
        JsonData = jsonData;
        ItemsCount = itemsCount;
        SnapshotTakenAtUtc = snapshotTakenAtUtc;
    }

    public static CartSnapshot Create(string jsonData, int itemsCount)
    {
        if (string.IsNullOrWhiteSpace(jsonData))
            throw new ArgumentException("Cart snapshot JSON data cannot be null or empty", nameof(jsonData));

        if (jsonData.Length > 50000) // حد أقصى 50KB
            throw new ArgumentException("Cart snapshot JSON data cannot exceed 50KB", nameof(jsonData));

        if (itemsCount < 0)
            throw new ArgumentException("Items count cannot be negative", nameof(itemsCount));

        // التحقق من صحة JSON
        try
        {
            System.Text.Json.JsonDocument.Parse(jsonData);
        }
        catch (System.Text.Json.JsonException ex)
        {
            throw new ArgumentException("Invalid JSON format", nameof(jsonData), ex);
        }

        return new CartSnapshot(jsonData, itemsCount, DateTime.UtcNow);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return JsonData;
        yield return ItemsCount;
        yield return SnapshotTakenAtUtc;
    }

    public override string ToString() => JsonData;

    public static implicit operator string(CartSnapshot snapshot) => snapshot.JsonData;
}
