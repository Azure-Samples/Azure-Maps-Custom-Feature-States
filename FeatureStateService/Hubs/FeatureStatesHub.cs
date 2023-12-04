﻿using FeatureStateService.Models;
using FeatureStateService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace FeatureStateService.Hubs;

[Authorize]
public partial class FeatureStatesHub : Hub
{
    private readonly ILogger<FeatureStatesHub> _logger;
    private readonly FeatureStatesStore _featureStateStore;

    public FeatureStatesHub(ILogger<FeatureStatesHub> logger, FeatureStatesStore featureStateStore)
    {
        _logger = logger;
        _featureStateStore = featureStateStore;
    }

    public override Task OnConnectedAsync()
    {
        var featureStates = _featureStateStore.GetAllFeatureStates();

        _logger.LogDebug($"Sending client with connection Id {Context.ConnectionId} snapshot with {featureStates.Count} feature states");

        return Clients.Client(Context.ConnectionId)
                      .SendAsync(Message.FeatureStateSnapshot.GetName(), featureStates);
    }
}