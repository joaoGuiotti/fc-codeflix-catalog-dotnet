using System.Text;
using System.Text.Json;
using FC.Codeflix.Catalog.EndToEndTests.Extensions;
using Microsoft.AspNetCore.WebUtilities;

namespace FC.Codeflix.Catalog.EndToEndTests.Common;

class SnakeCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
        => name.ToSnakeCase();
}

public class ApiClient
{
    private readonly HttpClient _httpClient;

    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public ApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = new SnakeCaseNamingPolicy(),
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<(HttpResponseMessage?, TOutput?)> Post<TOutput>(string uri, object payload)
        where TOutput : class
    {
        var stringContent = new StringContent(
            JsonSerializer.Serialize(payload, _jsonSerializerOptions),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _httpClient.PostAsync(uri, stringContent);
        var output = await GetOutput<TOutput>(response);

        return (response, output);
    }

    public async Task<(HttpResponseMessage?, TOutput?)> Get<TOutput>(
        string url,
        object? queryStringParametersObject = null
    )
        where TOutput : class
    {
        var finalUrl = PrepareGetRoute(url, queryStringParametersObject);
        var response = await _httpClient.GetAsync(finalUrl);
        var output = await GetOutput<TOutput>(response);
        return (response, output);
    }

    private string PrepareGetRoute(
        string url,
        object? queryStringParametersObject
    )
    {
        if (queryStringParametersObject is null)
            return url;
        var parametersJson = JsonSerializer.Serialize(
            queryStringParametersObject,
            _jsonSerializerOptions
        );
        var parametersDictionary = Newtonsoft.Json.JsonConvert
            .DeserializeObject<Dictionary<string, string>>(parametersJson);
        return QueryHelpers.AddQueryString(url, parametersDictionary!);
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
           JsonSerializer.Serialize(payload, _jsonSerializerOptions),
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

