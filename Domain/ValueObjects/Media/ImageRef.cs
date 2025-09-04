using Domain.Common;

namespace Domain.ValueObjects.Media;

/// <summary>
/// كائن قيمة لمرجع الصورة
/// </summary>
public sealed class ImageRef : ValueObject
{
    public string Url { get; private set; }
    public int? Width { get; private set; }
    public int? Height { get; private set; }
    public string? MimeType { get; private set; }

    private ImageRef(string url, int? width, int? height, string? mimeType)
    {
        Url = url;
        Width = width;
        Height = height;
        MimeType = mimeType;
    }

    public static ImageRef Create(string url, int? width = null, int? height = null, string? mimeType = null)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Image URL cannot be null or empty", nameof(url));

        if (url.Length > 500)
            throw new ArgumentException("Image URL cannot exceed 500 characters", nameof(url));

        if (width.HasValue && width <= 0)
            throw new ArgumentException("Width must be positive", nameof(width));

        if (height.HasValue && height <= 0)
            throw new ArgumentException("Height must be positive", nameof(height));

        if (!string.IsNullOrWhiteSpace(mimeType) && mimeType.Length > 100)
            throw new ArgumentException("MIME type cannot exceed 100 characters", nameof(mimeType));

        return new ImageRef(url.Trim(), width, height, 
            string.IsNullOrWhiteSpace(mimeType) ? null : mimeType.Trim());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Url;
        yield return Width;
        yield return Height;
        yield return MimeType;
    }

    public override string ToString() => Url;

    public static implicit operator string(ImageRef imageRef) => imageRef.Url;
}
