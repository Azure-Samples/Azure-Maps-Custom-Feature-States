using Newtonsoft.Json;

namespace FeatureStateDb;

public class FeatureStateUpdate
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; } = "";

    [JsonProperty(PropertyName = "partitionKey")]
    public string PartitionKey { get; set; } = Microsoft.Azure.Cosmos.PartitionKey.Null.ToString();

    [JsonProperty(PropertyName = "featureState")]
    public string FeatureState { get; set; } = "";

    [JsonProperty(PropertyName = "updateTimestamp")]
    public DateTime UpdateTimestamp { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}