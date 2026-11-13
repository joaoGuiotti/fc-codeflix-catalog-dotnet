using System.Text.Json;
using System.Text.Json.Serialization;

namespace FC.Codeflix.Catalog.Infra.Messaging.Configuration;

public static class JsonSerializerConfiguration
{
    public static JsonSerializerOptions ConfigureSnakeCaseJson(this JsonSerializerOptions options)
    {
        options.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
        options.DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower;
        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        return options;
    }
}

