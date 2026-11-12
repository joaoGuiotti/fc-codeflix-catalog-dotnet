using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using FC.Codeflix.Catalog.Infra.Messaging.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace FC.Codeflix.Catalog.EndToEndTests.Common;

public class ApiClient
{
    private readonly HttpClient _httpClient;

    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public ApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }.ConfigureJson();
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

    public async Task<(HttpResponseMessage?, TOutput?)> PostFormData<TOutput>(
        string route,
        object payload
    )
        where TOutput : class
    {
        var response = await _httpClient.PostAsync(
            route,
            PrepareFormData(payload)
        );
        var output = await GetOutput<TOutput>(response);
        return (response, output);
    }

    private HttpContent PrepareFormData(object payload)
    {
        if (payload is MultipartFormDataContent multipartContent)
            return multipartContent;

        var formData = new MultipartFormDataContent();

        if (payload is FileInput fileInputPayload)
        {
            var streamContent = new StreamContent(fileInputPayload.FileStream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(fileInputPayload.ContentType);
            formData.Add(streamContent, "media_file", $"file.{fileInputPayload.Extension}");
            return formData;
        }

        var properties = payload.GetType().GetProperties();
        foreach (var property in properties)
        {
            var attribute = property.GetCustomAttribute<FromFormAttribute>();
            var key = attribute?.Name ?? property.Name;
            var value = property.GetValue(payload);

            if (value is IFormFile formFile)
            {
                var streamContent = new StreamContent(formFile.OpenReadStream());
                if (!string.IsNullOrWhiteSpace(formFile.ContentType))
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue(formFile.ContentType);
                formData.Add(streamContent, key, formFile.FileName);
            }
            else if (value is FileInput fileInput)
            {
                var streamContent = new StreamContent(fileInput.FileStream);
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(fileInput.ContentType);
                formData.Add(streamContent, key, $"file.{fileInput.Extension}");
            }
            else if (value is string strValue)
            {
                formData.Add(new StringContent(strValue), key);
            }
        }

        return formData;
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

