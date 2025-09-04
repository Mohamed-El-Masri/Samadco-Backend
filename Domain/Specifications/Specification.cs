using System.Linq.Expressions;

namespace Domain.Specifications;

/// <summary>
/// واجهة نمط Specification لتعريف مواصفات مرشحات غنية
/// </summary>
public interface ISpecification<T>
{
    /// <summary>
    /// معيار التصفية
    /// </summary>
    Expression<Func<T, bool>> Criteria { get; }
}

/// <summary>
/// فئة أساسية لتنفيذ نمط Specification
/// </summary>
public abstract class Specification<T> : ISpecification<T>
{
    public abstract Expression<Func<T, bool>> Criteria { get; }

    /// <summary>
    /// دمج المواصفات باستخدام AND
    /// </summary>
    public static Specification<T> operator &(Specification<T> left, Specification<T> right)
    {
        return new AndSpecification<T>(left, right);
    }

    /// <summary>
    /// دمج المواصفات باستخدام OR
    /// </summary>
    public static Specification<T> operator |(Specification<T> left, Specification<T> right)
    {
        return new OrSpecification<T>(left, right);
    }

    /// <summary>
    /// نفي المواصفة باستخدام NOT
    /// </summary>
    public static Specification<T> operator !(Specification<T> specification)
    {
        return new NotSpecification<T>(specification);
    }

    /// <summary>
    /// التحقق من استيفاء كائن للمواصفة
    /// </summary>
    public virtual bool IsSatisfiedBy(T entity)
    {
        var predicate = Criteria.Compile();
        return predicate(entity);
    }
}

/// <summary>
/// مواصفة AND المركبة
/// </summary>
internal sealed class AndSpecification<T> : Specification<T>
{
    private readonly Specification<T> _left;
    private readonly Specification<T> _right;

    public AndSpecification(Specification<T> left, Specification<T> right)
    {
        _left = left;
        _right = right;
    }

    public override Expression<Func<T, bool>> Criteria =>
        CombineExpressions(_left.Criteria, _right.Criteria, Expression.AndAlso);

    private static Expression<Func<T, bool>> CombineExpressions(
        Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right,
        Func<Expression, Expression, Expression> combiner)
    {
        var parameter = Expression.Parameter(typeof(T));

        var leftVisitor = new ReplaceExpressionVisitor(left.Parameters[0], parameter);
        var leftExpression = leftVisitor.Visit(left.Body);

        var rightVisitor = new ReplaceExpressionVisitor(right.Parameters[0], parameter);
        var rightExpression = rightVisitor.Visit(right.Body);

        return Expression.Lambda<Func<T, bool>>(
            combiner(leftExpression!, rightExpression!), parameter);
    }
}

/// <summary>
/// مواصفة OR المركبة
/// </summary>
internal sealed class OrSpecification<T> : Specification<T>
{
    private readonly Specification<T> _left;
    private readonly Specification<T> _right;

    public OrSpecification(Specification<T> left, Specification<T> right)
    {
        _left = left;
        _right = right;
    }

    public override Expression<Func<T, bool>> Criteria =>
        CombineExpressions(_left.Criteria, _right.Criteria, Expression.OrElse);

    private static Expression<Func<T, bool>> CombineExpressions(
        Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right,
        Func<Expression, Expression, Expression> combiner)
    {
        var parameter = Expression.Parameter(typeof(T));

        var leftVisitor = new ReplaceExpressionVisitor(left.Parameters[0], parameter);
        var leftExpression = leftVisitor.Visit(left.Body);

        var rightVisitor = new ReplaceExpressionVisitor(right.Parameters[0], parameter);
        var rightExpression = rightVisitor.Visit(right.Body);

        return Expression.Lambda<Func<T, bool>>(
            combiner(leftExpression!, rightExpression!), parameter);
    }
}

/// <summary>
/// مواصفة NOT
/// </summary>
internal sealed class NotSpecification<T> : Specification<T>
{
    private readonly Specification<T> _specification;

    public NotSpecification(Specification<T> specification)
    {
        _specification = specification;
    }

    public override Expression<Func<T, bool>> Criteria
    {
        get
        {
            var parameter = Expression.Parameter(typeof(T));
            var visitor = new ReplaceExpressionVisitor(_specification.Criteria.Parameters[0], parameter);
            var expression = visitor.Visit(_specification.Criteria.Body);

            return Expression.Lambda<Func<T, bool>>(Expression.Not(expression!), parameter);
        }
    }
}

/// <summary>
/// زائر لاستبدال المعاملات في التعبيرات
/// </summary>
internal sealed class ReplaceExpressionVisitor : ExpressionVisitor
{
    private readonly Expression _oldValue;
    private readonly Expression _newValue;

    public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
    {
        _oldValue = oldValue;
        _newValue = newValue;
    }

    public override Expression? Visit(Expression? node)
    {
        return node == _oldValue ? _newValue : base.Visit(node);
    }
}
