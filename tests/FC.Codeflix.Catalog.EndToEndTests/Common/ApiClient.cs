using Microsoft.VisualStudio.TestPlatform.Utilities;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace FC.Codeflix.Catalog.EndToEndTests.Common;

public class ApiClient
{
    private readonly HttpClient _httpClient;

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ApiClient(HttpClient httpClient)
        => _httpClient = httpClient;

    public async Task<(HttpResponseMessage?, TOutput?)> Post<TOutput>(string uri, object payload)
    {
        var stringContent = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _httpClient.PostAsync(uri, stringContent);
        var output = await response.Content
            .ReadFromJsonAsync<TOutput>(_jsonSerializerOptions);

        return (response, output);
    }

    public async Task<(HttpResponseMessage?, TOutput?)> Get<TOutput>(string url)
    {
        var response = await _httpClient.GetAsync(url);
        var output = await response.Content
                   .ReadFromJsonAsync<TOutput>(_jsonSerializerOptions);
        return (response, output);
    }

    public async Task<(HttpResponseMessage?, TOutput?)> Delete<TOutput>(string url)
            where TOutput : class
    {
        var response = await _httpClient.DeleteAsync(url);
        var outputString = await response.Content
                   .ReadAsStringAsync();

        TOutput? output = string.IsNullOrWhiteSpace(outputString)
            ? null
            : JsonSerializer.Deserialize<TOutput>(outputString, _jsonSerializerOptions);

        return (response, output);
    }
}
