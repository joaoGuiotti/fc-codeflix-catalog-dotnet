using FC.Codeflix.Catalog.Domain.Events;

namespace FC.Codeflix.Catalog.Infra.Messaging.Configuration;

public static class EventsMapping
{
    private readonly static Dictionary<string, string> _mapping = new()
    {
        { typeof(VideoUploadedEvent).Name, "videos.create" },
    };

    public static string GetRoutingKey<T>()
       => _mapping.TryGetValue(typeof(T).Name, out var routingKey)
            ? routingKey : string.Empty;
}
