using FeatureStateService.Models;

namespace FeatureStateService.Services;

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