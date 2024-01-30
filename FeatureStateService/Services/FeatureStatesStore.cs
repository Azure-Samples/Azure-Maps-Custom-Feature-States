using FeatureStateDb;
using FeatureStateService.Hubs;
using FeatureStateService.Models;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace FeatureStateService.Services;

/// <summary>
/// Represents a latest-value store of feature states for all features. Updates are propagated to connected clients and to the database.
/// </summary>
public class FeatureStatesStore
{
    private readonly ConcurrentDictionary<string, string> _featureStatesById;
    private readonly IHubContext<FeatureStatesHub> _hub;
    private readonly FeatureStateRepository _featureStateRepository;
    private readonly FeaturesStore _featuresStore;

    public FeatureStatesStore(IHubContext<FeatureStatesHub> hub, FeatureStateRepository featureStateRepository, FeaturesStore featuresStore)
    {
        _hub = hub;
        _featureStateRepository = featureStateRepository;
        _featuresStore = featuresStore;

        _featureStatesById = new(_featureStateRepository.GetFeatureStateUpdatesAsync().Result.ToDictionary(update => update.Id, update => update.FeatureState));
    }

    public Dictionary<string, string> GetAllFeatureStates()
    {
        return _featureStatesById.ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    public string this[string id]
    {
        get
        {
            return _featureStatesById[id];
        }
        set
        {
            if (!_featuresStore.FeatureIds.Contains(id))
            {
                throw new KeyNotFoundException();
            }

            _featureStatesById[id] = value;
            _featureStateRepository.PersistFeatureStateUpdateAsync(new FeatureStateUpdate { Id = id, FeatureState = value, UpdateTimestamp = DateTime.UtcNow });
            _hub.Clients.All.SendAsync(Message.FeatureStateChange.GetName(), id, value);
        }
    }
}
