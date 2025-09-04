namespace Domain.Common;

/// <summary>
/// مساعد لاسترجاع النصوص متعددة اللغات
/// يستخدم نمط الحقول المزدوجة بدلاً من Value Objects للترجمة
/// </summary>
public static class LocalizationHelper
{
    /// <summary>
    /// استرجاع النص المناسب حسب الثقافة المطلوبة
    /// </summary>
    /// <param name="en">النص الإنجليزي (إجباري)</param>
    /// <param name="ar">النص العربي (اختياري)</param>
    /// <param name="culture">رمز الثقافة (مثل: ar-SA, en-US)</param>
    /// <returns>النص المناسب للثقافة المطلوبة</returns>
    public static string Localize(string? en, string? ar, string culture)
    {
        // إذا كانت الثقافة عربية وتوجد ترجمة عربية، استخدمها
        if (culture.StartsWith("ar", StringComparison.OrdinalIgnoreCase))
        {
            return string.IsNullOrWhiteSpace(ar) ? en ?? string.Empty : ar!;
        }
        
        // وإلا استخدم النص الإنجليزي
        return en ?? string.Empty;
    }

    /// <summary>
    /// استرجاع النص المناسب حسب الثقافة مع قيمة افتراضية
    /// </summary>
    /// <param name="en">النص الإنجليزي</param>
    /// <param name="ar">النص العربي</param>
    /// <param name="culture">رمز الثقافة</param>
    /// <param name="defaultValue">القيمة الافتراضية إذا كانت جميع النصوص فارغة</param>
    /// <returns>النص المناسب أو القيمة الافتراضية</returns>
    public static string LocalizeWithDefault(string? en, string? ar, string culture, string defaultValue)
    {
        var result = Localize(en, ar, culture);
        return string.IsNullOrWhiteSpace(result) ? defaultValue : result;
    }

    /// <summary>
    /// التحقق من وجود ترجمة للثقافة المطلوبة
    /// </summary>
    /// <param name="en">النص الإنجليزي</param>
    /// <param name="ar">النص العربي</param>
    /// <param name="culture">رمز الثقافة</param>
    /// <returns>true إذا كان هناك نص متاح للثقافة المطلوبة</returns>
    public static bool HasTranslation(string? en, string? ar, string culture)
    {
        if (culture.StartsWith("ar", StringComparison.OrdinalIgnoreCase))
        {
            return !string.IsNullOrWhiteSpace(ar) || !string.IsNullOrWhiteSpace(en);
        }
        
        return !string.IsNullOrWhiteSpace(en);
    }

    /// <summary>
    /// الحصول على جميع الترجمات المتاحة
    /// </summary>
    /// <param name="en">النص الإنجليزي</param>
    /// <param name="ar">النص العربي</param>
    /// <returns>قاموس بالترجمات المتاحة</returns>
    public static Dictionary<string, string> GetAllTranslations(string? en, string? ar)
    {
        var translations = new Dictionary<string, string>();
        
        if (!string.IsNullOrWhiteSpace(en))
            translations["en"] = en!;
            
        if (!string.IsNullOrWhiteSpace(ar))
            translations["ar"] = ar!;
            
        return translations;
    }
}
