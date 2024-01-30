using FeatureStateService.Config;
using FeatureStateService.Models;
using FeatureStateService.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FeatureStateService.Controllers;

/// <summary>
/// A controller with view support for handling browser requests for the default/Home route.
/// Supports actions for viewing the map and editing feature states from a browser.
/// </summary>
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly FeaturesStore _featuresStore;
    private readonly AzureMapsConfig _azureMapsConfig;
    private readonly EditFeatureStatesConfig _editFeatureStatesConfig;

    public HomeController(ILogger<HomeController> logger, FeaturesStore featureStore, AzureMapsConfig azureMapsConfig, EditFeatureStatesConfig editFeatureStatesConfig)
    {
        _logger = logger;
        _featuresStore = featureStore;
        _azureMapsConfig = azureMapsConfig;
        _editFeatureStatesConfig = editFeatureStatesConfig;
    }

    public IActionResult Index() => View();

    public IActionResult EditFeatureStates()
    {
        ViewBag.FeatureStates = _editFeatureStatesConfig.FeatureStates;

        ViewBag.Features = _featuresStore.FeatureCollection
                                         .Features
                                         .GroupBy(feature => feature.Id)
                                         .ToDictionary(group => group.Key, group => group.First().Properties.Name)
                                         .OrderBy(pair => pair.Value)
                                         .ToList();

        return View();
    }

    public IActionResult ViewMap()
    {
        ViewBag.Domain = _azureMapsConfig.Domain;
        ViewBag.ClientId = _azureMapsConfig.ClientId;
        ViewBag.TokenUrl = _azureMapsConfig.TokenUrl;
        ViewBag.MapConfigurationId = _azureMapsConfig.MapConfigurationId;
        ViewBag.TilesetId = _azureMapsConfig.TilesetId;
        ViewBag.APIVersion = _azureMapsConfig.APIVersion;
        ViewBag.SourceLayer = _azureMapsConfig.SourceLayer;

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}