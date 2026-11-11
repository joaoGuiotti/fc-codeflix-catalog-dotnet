using System.Text.Json;
using System.Text.Json.Serialization;

namespace FC.Codeflix.Catalog.Infra.Messaging.Configuration;

public static class JsonSerializerConfiguration
{
    private static readonly JsonSerializerOptions _defaultOptions =
        new JsonSerializerOptions().ConfigureJson();

    public static JsonSerializerOptions ConfigureJson(this JsonSerializerOptions options)
    {
        options.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
        options.DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower;
        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        return options;
    }
}

