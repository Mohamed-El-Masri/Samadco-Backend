namespace Domain.Enums.Payments;

/// <summary>
/// طرق الدفع
/// </summary>
public enum PaymentMethod
{
    /// <summary>
    /// بطاقة ائتمان
    /// </summary>
    CreditCard,
    
    /// <summary>
    /// تحويل بنكي
    /// </summary>
    BankTransfer,
    
    /// <summary>
    /// نقداً عند الاستلام
    /// </summary>
    CashOnDelivery,
    
    /// <summary>
    /// محفظة إلكترونية
    /// </summary>
    DigitalWallet
}
