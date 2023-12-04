namespace FeatureStateService.Controllers;

using FeatureStateService.Services;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

[Route("api/features")]
[ApiController]
public class FeaturesApiController : ControllerBase
{
    private readonly ILogger<FeaturesApiController> _logger;
    private readonly FeaturesStore _featuresStore;

    public FeaturesApiController(ILogger<FeaturesApiController> logger, FeaturesStore featuresStore)
    {
        _logger = logger;
        _featuresStore = featuresStore;
    }

    [HttpGet]
    public ActionResult<FeatureCollection> GetFeatures() => Ok(_featuresStore.FeatureCollection);
}