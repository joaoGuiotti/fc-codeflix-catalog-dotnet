using FC.Codeflix.Catalog.Domain.SeedWork;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Extensions;

public static class QueryableSearchExtensions
{
    private const int MaxPerPage = 100;

    public static IQueryable<T> ApplyFilter<T>(
        this IQueryable<T> query,
        string? search,
        params Expression<Func<T, string?>>[] fields
    ) where T : AggregateRoot
    {
        if (string.IsNullOrWhiteSpace(search) || fields.Length == 0)
            return query;

        var parameter = Expression.Parameter(typeof(T), "x");
        Expression<Func<string>> searchAccessor = () => search;
        var searchValue = searchAccessor.Body;
        var containsMethod = typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!;

        // x => x.A.Contains(term) || x.B.Contains(term) ...
        // Sem ToLower e sem null-check: o SQL já trata NULL, e a comparação
        // case-insensitive fica a cargo do collation do banco (índices continuam utilizáveis).
        var body = fields
            .Select(field => (Expression)Expression.Call(
                ReplaceParameter(field.Body, field.Parameters[0], parameter),
                containsMethod,
                searchValue
                ))
            .Aggregate((left, right) => Expression.OrElse(left, right));

        return query.Where(Expression.Lambda<Func<T, bool>>(body, parameter));
    }

    public static IOrderedQueryable<T> ApplyOrder<T, TDefaultKey>(
        this IQueryable<T> query,
        string? sort,
        ESearchOrder? sortDir,
        Expression<Func<T, TDefaultKey>> defaultOrder,
        params Expression<Func<T, object?>>[] allowedFields
    ) where T : AggregateRoot
    {
        var property = ResolveSortProperty<T>(sort, allowedFields);

        IOrderedQueryable<T> ordered;

        if (property is null)
        {
            // Sem sort válido: usa a ordenação padrão, respeitando sortDir (padrão = Desc).
            var descending = (sortDir ?? ESearchOrder.Desc) == ESearchOrder.Desc;
            ordered = descending
                ? query.OrderByDescending(defaultOrder)
                : query.OrderBy(defaultOrder);
        }
        else
        {
            ordered = OrderByProperty(query, property, sortDir == ESearchOrder.Desc);
        }

        // Desempate por Id para que a paginação seja estável quando há valores repetidos.
        return property?.Name == nameof(AggregateRoot.Id)
            ? ordered
            : ordered.ThenBy(x => x.Id);
    }

    public static async Task<SearchOutput<T>> ToSearchOutputAsync<T>(
        this IQueryable<T> query,
        SearchInput input,
        CancellationToken cancellationToken
    ) where T : AggregateRoot
    {
        var page = Math.Max(input.Page, 1);
        var perPage = Math.Clamp(input.PerPage, 1, MaxPerPage);

        var total = await query.CountAsync(cancellationToken);

        IReadOnlyList<T> items = total == 0
            ? []
            : await query
                .Skip((page - 1) * perPage)
                .Take(perPage)
                .ToListAsync(cancellationToken);

        return new SearchOutput<T>(
            perPage: perPage,
            currentPage: page,
            total: total,
            items: items
        );
    }

    private static PropertyInfo? ResolveSortProperty<T>(
        string? sort,
        Expression<Func<T, object?>>[] allowedFields)
    {
        if (string.IsNullOrWhiteSpace(sort))
            return null;

        if (allowedFields.Length > 0)
        {
            var isAllowed = allowedFields
                .Select(GetMemberName)
                .Any(name => string.Equals(name, sort, StringComparison.OrdinalIgnoreCase));

            if (!isAllowed)
                return null;
        }

        // FirstOrDefault evita a AmbiguousMatchException do GetProperty com IgnoreCase.
        return typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .FirstOrDefault(p => string.Equals(p.Name, sort, StringComparison.OrdinalIgnoreCase));
    }

    private static IOrderedQueryable<T> OrderByProperty<T>(
        IQueryable<T> query,
        PropertyInfo property,
        bool descending)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var lambda = Expression.Lambda(Expression.Property(parameter, property), parameter);

        var methodName = descending
            ? nameof(Queryable.OrderByDescending)
            : nameof(Queryable.OrderBy);

        var call = Expression.Call(
            typeof(Queryable),
            methodName,
            [typeof(T), property.PropertyType],
            query.Expression,
            Expression.Quote(lambda)
        );

        return (IOrderedQueryable<T>)query.Provider.CreateQuery<T>(call);
    }

    private static string? GetMemberName<T>(Expression<Func<T, object?>> expression)
        => expression.Body switch
        {
            MemberExpression member => member.Member.Name,
            UnaryExpression { Operand: MemberExpression member } => member.Member.Name,
            _ => null
        };

    private static Expression ReplaceParameter(
        Expression expression,
        ParameterExpression oldParameter,
        ParameterExpression newParameter
    ) => new ParameterReplacer(oldParameter, newParameter).Visit(expression);

    private sealed class ParameterReplacer(
        ParameterExpression oldParameter,
        ParameterExpression newParameter
    ) : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node)
            => node == oldParameter ? newParameter : base.VisitParameter(node);
    }
}
