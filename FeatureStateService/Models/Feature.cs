using System.Text.Json.Serialization;

namespace FeatureStateService.Models;

public class Feature
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("geometry")]
    public Geometry Geometry { get; set; } = new();

    [JsonPropertyName("properties")]
    public Properties Properties { get; set; } = new();

    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("featureType")]
    public string? FeatureType { get; set; }
}

public class Geometry
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("coordinates")]
    public List<object> Coordinates { get; set; } = new();
}

public class Link
{
    [JsonPropertyName("href")]
    public string? Href { get; set; }

    [JsonPropertyName("rel")]
    public string? Rel { get; set; }
}

public class Properties
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("levelOrdinal")]
    public int LevelOrdinal { get; set; }

    [JsonPropertyName("layerName")]
    public string? LayerName { get; set; }

    [JsonPropertyName("levelId")]
    public string? LevelId { get; set; }

    [JsonPropertyName("azureMapsIntId")]
    public int AzureMapsIntId { get; set; }
}

public class FeatureCollection
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("ontology")]
    public string? Ontology { get; set; }

    [JsonPropertyName("features")]
    public List<Feature> Features { get; set; } = new();

    [JsonPropertyName("numberReturned")]
    public int NumberReturned { get; set; }

    [JsonPropertyName("links")]
    public List<Link> Links { get; set; } = new();
}

