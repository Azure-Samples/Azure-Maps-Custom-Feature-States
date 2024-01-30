using FeatureStateDb.Config;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;
using Container = Microsoft.Azure.Cosmos.Container;

namespace FeatureStateDb;

/// <summary>
/// Reads the latest feature states of all features from CosmosDB and writes updates to it.
/// </summary>
public class FeatureStateRepository
{
    private readonly ILogger<FeatureStateRepository> _logger;
    private readonly Container _container;

    private const string ContainerId = "FeatureStates";

    public FeatureStateRepository(ILogger<FeatureStateRepository> logger, DatabaseConfig config)
    {
        _logger = logger;
        _container = new CosmosClientBuilder(config.ConnectionString).Build()
                                                                     .GetDatabase(config.Name)
                                                                     .GetContainer(ContainerId);
    }

    public async Task<List<FeatureStateUpdate>> GetFeatureStateUpdatesAsync()
    {
        List<FeatureStateUpdate> result = new();

        var query = _container.GetItemLinqQueryable<FeatureStateUpdate>().ToFeedIterator();

        do
        {
            var nextResponse = await query.ReadNextAsync();
            result.AddRange(nextResponse);

        } while (query.HasMoreResults);

        return result;
    }

    public async void PersistFeatureStateUpdateAsync(FeatureStateUpdate update)
    {
        _logger.LogDebug($"Persisting item {update}");

        await _container.UpsertItemAsync(update);
    }
}
