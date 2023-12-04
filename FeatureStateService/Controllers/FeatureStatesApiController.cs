namespace FeatureStateService.Controllers;

using FeatureStateService.Services;
using Microsoft.AspNetCore.Mvc;

[Route("api/featurestates")]
[ApiController]
public class FeatureStatesApiController : ControllerBase
{
    private readonly ILogger<FeatureStatesApiController> _logger;
    private readonly FeatureStatesStore _featureStatesStore;

    public FeatureStatesApiController(ILogger<FeatureStatesApiController> logger, FeatureStatesStore featureStatesStore)
    {
        _logger = logger;
        _featureStatesStore = featureStatesStore;
    }

    [HttpGet]
    public ActionResult<Dictionary<string, string>> GetFeatureStates() => Ok(_featureStatesStore.GetAllFeatureStates());

    [HttpGet("{id}")]
    public ActionResult<string> GetFeatureState(string id)
    {
        try
        {
            return Ok(_featureStatesStore[id]);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    public record class FeatureStatePostData(string Id, string Value);

    [HttpPost]
    public ActionResult PostFeatureState(FeatureStatePostData data)
    {
        try
        {
            _featureStatesStore[data.Id] = data.Value;

            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}