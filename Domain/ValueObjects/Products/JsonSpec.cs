using Domain.Common;
using System.Text.Json;

namespace Domain.ValueObjects.Products;

/// <summary>
/// كائن قيمة لمواصفات المنتج بصيغة JSON
/// </summary>
public sealed class JsonSpec : ValueObject
{
    private JsonDocument? _value;
    
    public JsonDocument Value => _value ??= JsonDocument.Parse(JsonString);
    public string JsonString { get; private set; } = default!;

    // Constructor فارغ لـ EF Core
    private JsonSpec()
    {
    }

    private JsonSpec(JsonDocument value, string jsonString)
    {
        _value = value;
        JsonString = jsonString;
    }

    public static JsonSpec Create(string jsonString)
    {
        if (string.IsNullOrWhiteSpace(jsonString))
            throw new ArgumentException("JSON specification cannot be null or empty", nameof(jsonString));

        if (jsonString.Length > 10000) // حد أقصى 10KB
            throw new ArgumentException("JSON specification cannot exceed 10000 characters", nameof(jsonString));

        try
        {
            var jsonDocument = JsonDocument.Parse(jsonString);
            return new JsonSpec(jsonDocument, jsonString);
        }
        catch (JsonException ex)
        {
            throw new ArgumentException("Invalid JSON format", nameof(jsonString), ex);
        }
    }

    public static JsonSpec Create(JsonDocument jsonDocument)
    {
        if (jsonDocument == null)
            throw new ArgumentNullException(nameof(jsonDocument));

        var jsonString = jsonDocument.RootElement.GetRawText();
        
        if (jsonString.Length > 10000)
            throw new ArgumentException("JSON specification cannot exceed 10000 characters");

        return new JsonSpec(jsonDocument, jsonString);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return JsonString;
    }

    public override string ToString() => JsonString;

    public static implicit operator string(JsonSpec jsonSpec) => jsonSpec.JsonString;

    public void Dispose()
    {
        Value?.Dispose();
    }
}
