using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
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
        where TOutput : class
    {
        var stringContent = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _httpClient.PostAsync(uri, stringContent);
        var output = await GetOutput<TOutput>(response);

        return (response, output);
    }

    public async Task<(HttpResponseMessage?, TOutput?)> Get<TOutput>(string url)
            where TOutput : class
    {
        var response = await _httpClient.GetAsync(url);
        var output = await GetOutput<TOutput>(response);
        return (response, output);
    }

    public async Task<(HttpResponseMessage?, TOutput?)> Delete<TOutput>(string url)
            where TOutput : class
    {
        var response = await _httpClient.DeleteAsync(url);
        var output = await GetOutput<TOutput>(response);

        return (response, output);
    }

    public async Task<(HttpResponseMessage?, TOutput?)> Put<TOutput>(string url, object payload)
           where TOutput : class
    {
        var stringContent = new StringContent(
           JsonSerializer.Serialize(payload),
           Encoding.UTF8,
           "application/json"
       );
        var response = await _httpClient.PutAsync(url, stringContent);
        var output = await GetOutput<TOutput>(response);

        return (response, output);
    }


    private async Task<TOutput?> GetOutput<TOutput>(HttpResponseMessage response)
        where TOutput : class
    {
        string outputString = await response.Content
                   .ReadAsStringAsync();
        return string.IsNullOrWhiteSpace(outputString)
           ? null
           : JsonSerializer.Deserialize<TOutput>(outputString, _jsonSerializerOptions);
    }
}
