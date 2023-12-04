using FeatureStateService.Config;
using FeatureStateService.Models;
using FeatureStateService.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FeatureStateService.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly FeaturesStore _featuresStore;
    private readonly AzureMapsConfig _azureMapsConfig;
    private readonly StyleConfig _styleConfig;

    public HomeController(ILogger<HomeController> logger, FeaturesStore featureStore, AzureMapsConfig azureMapsConfig, StyleConfig styleConfig)
    {
        _logger = logger;
        _featuresStore = featureStore;
        _azureMapsConfig = azureMapsConfig;
        _styleConfig = styleConfig;
    }

    public IActionResult Index() => View();

    public IActionResult EditFeatureStates()
    {
        ViewBag.FeatureStates = _styleConfig.StyleRules
                                            .ConvertAll(styleRule => styleRule.FeatureState);

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
        ViewBag.APIVersion = _azureMapsConfig.APIVersion;
        ViewBag.SourceLayer = _azureMapsConfig.SourceLayer;
        ViewBag.StyleRules = string.Join(", ", _styleConfig.StyleRules
                                                           .SelectMany(rule => new[] { rule.FeatureState, rule.Color })
                                                           .Select(rule => @$"'{rule}'"));

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}