namespace FeatureStateService.Config;

public record EditFeatureStatesConfig
{
    public List<string> FeatureStates { get; set; } = new();
}