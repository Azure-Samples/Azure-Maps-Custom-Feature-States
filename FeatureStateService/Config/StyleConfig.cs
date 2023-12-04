namespace FeatureStateService.Config;

public record class StyleRule(string FeatureState, string Color);

public record class StyleConfig
{
    public List<StyleRule> StyleRules { get; set; } = new();
}