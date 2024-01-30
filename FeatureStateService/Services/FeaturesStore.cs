using FeatureStateService.Models;

namespace FeatureStateService.Services;

/// <summary>
/// Maintains a static store all features for the configured map. Used for validating featureIds for feature state updates and for retrieving
/// friendly names for features.
/// </summary>
public class FeaturesStore
{
    public FeatureCollection FeatureCollection { get; private set; }

    public IReadOnlySet<string> FeatureIds { get; private set; }

    public FeaturesStore(FeatureDownloader downloader)
    {
        FeatureCollection = downloader.GetCollectionAsync().Result;

        HashSet<string> allowedGeometryTypes = new(StringComparer.OrdinalIgnoreCase) { "Polygon", "MultiPolygon" };
        FeatureCollection.Features.RemoveAll(feature => !allowedGeometryTypes.Contains(feature.Geometry.Type ?? ""));
        FeatureCollection.NumberReturned = FeatureCollection.Features.Count;

        FeatureIds = FeatureCollection.Features.Select(Feature => Feature.Id).ToHashSet();
    }
}