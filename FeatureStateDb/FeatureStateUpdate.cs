using Newtonsoft.Json;

namespace FeatureStateDb;

/// <summary>
/// Represents a change to the feature state of a feature or its latest feature state.
/// </summary>
public class FeatureStateUpdate
{
    /// <summary>Gets or sets the featureId of the feature.</summary>
    /// <value>The featureId; used as a unique item name in CosmosDB .</value>
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; } = "";

    /// <summary>Gets or sets the partition key for the item.</summary>
    /// <value>The partition key for the item in CosmosDB. Currently set to <see langword="null" />; you may wish to change this
    /// if you'd like to store feature states for multiple maps within a single CosmosDB container.</value>
    [JsonProperty(PropertyName = "partitionKey")]
    public string PartitionKey { get; set; } = Microsoft.Azure.Cosmos.PartitionKey.Null.ToString();

    /// <summary>Gets or sets the feature state of the feature.</summary>
    /// <value>The feature state of the feature.</value>
    [JsonProperty(PropertyName = "featureState")]
    public string FeatureState { get; set; } = "";

    /// <summary>Gets or sets the timestamp of this update.</summary>
    /// <value>The update timestamp in UTC.</value>
    [JsonProperty(PropertyName = "updateTimestamp")]
    public DateTime UpdateTimestamp { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}