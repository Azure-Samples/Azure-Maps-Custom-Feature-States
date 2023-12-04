using FeatureStateService.Config;
using FeatureStateService.Models;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Web;

namespace FeatureStateService.Services;

public class FeatureDownloader
{
    private readonly FeatureDownloaderConfig _featureDownloaderConfig;
    private readonly AzureMapsConfig _azureMapsConfig;
    private readonly AzureMapsTokenProvider _tokenProvider;

    private const int MaxFeaturesPerRequest = 100;

    public FeatureDownloader(FeatureDownloaderConfig featureDownloaderConfig, AzureMapsConfig azureMapsConfig, AzureMapsTokenProvider tokenProvider)
    {
        _featureDownloaderConfig = featureDownloaderConfig;
        _azureMapsConfig = azureMapsConfig;
        _tokenProvider = tokenProvider;
    }

    public async Task<FeatureCollection> GetCollectionAsync()
    {
        var cacheFileName = $"azuremaps_features_{DateTime.Today:yyyy_MM_dd}_{_azureMapsConfig.DatasetId}_{_azureMapsConfig.SourceLayer}_{_azureMapsConfig.APIVersion}.json";
        var cachePath = Path.Combine(Path.GetTempPath(), cacheFileName);

        if (_featureDownloaderConfig.AllowUseLocalCache && Path.Exists(cachePath))
        {
            return JsonSerializer.Deserialize<FeatureCollection>(File.ReadAllText(cachePath))!;
        }

        var result = await DownloadCollectionAsync();

        await File.WriteAllTextAsync(cachePath, JsonSerializer.Serialize(result));

        return result;
    }

    private async Task<FeatureCollection> DownloadCollectionAsync()
    {
        using HttpClient client = new(new HttpClientHandler
        {
            AllowAutoRedirect = false,
        });

        var token = await _tokenProvider.GetAccessTokenAsync();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
        client.DefaultRequestHeaders.Add("x-ms-client-id", _azureMapsConfig.ClientId);

        var requestUri = $"{_azureMapsConfig.Domain}/features/datasets/{_azureMapsConfig.DatasetId}/collections/{_azureMapsConfig.SourceLayer}/items";

        var result = await DownloadCollectionPagesAsync(client, GetRequestUri(requestUri));

        return !result.Any()
            ? new()
            : new FeatureCollection
            {
                Type = result.First().Type,
                Ontology = result.First().Ontology,
                NumberReturned = result.Sum(result => result.Features.Count),
                Features = result.SelectMany(result => result.Features).ToList(),
            };
    }

    private async Task<List<FeatureCollection>> DownloadCollectionPagesAsync(HttpClient client, Uri requestUri)
    {
        HttpRequestMessage request = new(HttpMethod.Get, requestUri);

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        List<FeatureCollection> result = new();

        var responseString = await response.Content.ReadAsStringAsync();

        var features = JsonSerializer.Deserialize<FeatureCollection>(responseString)!;
        result.Add(features);

        var next = features.Links.SingleOrDefault(link => link.Rel == "next");

        if (next != null && next.Href != null)
        {
            result.AddRange(await DownloadCollectionPagesAsync(client, GetRequestUri(next.Href)));
        }

        return result;
    }

    private Uri GetRequestUri(string baseUri)
    {
        var queryString = HttpUtility.ParseQueryString(new Uri(baseUri).Query);

        if (string.IsNullOrEmpty(queryString.Get("api-version")))
        {
            queryString.Set("api-version", _azureMapsConfig.APIVersion);
        }

        if (string.IsNullOrEmpty(queryString.Get("limit")))
        {
            queryString.Set("limit", MaxFeaturesPerRequest.ToString());
        }

        UriBuilder uriBuilder = new(baseUri)
        {
            Query = queryString.ToString()
        };

        return uriBuilder.Uri;
    }
}
