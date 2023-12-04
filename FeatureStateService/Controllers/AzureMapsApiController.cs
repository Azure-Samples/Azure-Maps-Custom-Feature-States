using FeatureStateService.Services;
using Microsoft.AspNetCore.Mvc;

namespace FeatureStateService.Controllers;

[Route("api/azuremaps")]
[ApiController]
public class AzureMapsApiController : ControllerBase
{
    private readonly AzureMapsTokenProvider _tokenProvider;

    public AzureMapsApiController(AzureMapsTokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    // GET: api/azuremaps/token
    [HttpGet("token")]
    public async Task<IActionResult> GetAzureMapsTokenAsync()
    {
        var token = await _tokenProvider.GetAccessTokenAsync();

        return Ok(token.Token);
    }
}
