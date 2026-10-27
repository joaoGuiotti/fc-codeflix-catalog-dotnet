namespace FC.Codeflix.Catalog.Application.Exceptions;

public class RelatedAggregateException : ApplicationException
{
    public RelatedAggregateException(string message)
        : base(message) { }

    public static void ThrowIfNotFound(
        IEnumerable<Guid> requestedIds,
        IEnumerable<Guid> foundIds,
        string aggregateName
    )
    {
        var notFoundIds = requestedIds.Except(foundIds).ToList();
        if (notFoundIds.Count > 0)
        {
            var notFoundIdsAsString = string.Join(", ", notFoundIds);
            throw new RelatedAggregateException(
                $"Related {aggregateName} id (or ids) not found: {notFoundIdsAsString}."
            );
        }
    }
}
