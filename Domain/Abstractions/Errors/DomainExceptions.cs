namespace Domain.Abstractions.Errors;

/// <summary>
/// استثناء انتهاك قاعدة من قواعد النطاق
/// </summary>
public sealed class DomainRuleViolationException : Exception
{
    public DomainRuleViolationException(string message) : base(message)
    {
    }

    public DomainRuleViolationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
/// استثناء عدم وجود الكيان المطلوب
/// </summary>
public sealed class EntityNotFoundDomainException : Exception
{
    public EntityNotFoundDomainException(string entityType, object id) 
        : base($"{entityType} with id '{id}' was not found.")
    {
        EntityType = entityType;
        Id = id;
    }

    public string EntityType { get; }
    public object Id { get; }
}

/// <summary>
/// استثناء تضارب التزامن
/// </summary>
public sealed class ConcurrencyDomainException : Exception
{
    public ConcurrencyDomainException(string message) : base(message)
    {
    }

    public ConcurrencyDomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
