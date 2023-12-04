using FeatureStateDb;
using FeatureStateDb.Config;
using FeatureStateService.Config;
using FeatureStateService.Hubs;
using FeatureStateService.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;


var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

// Add services to the container.
services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApp(configuration.GetSection("AzureAd"));

services.AddSingleton(configuration.GetSection("AzureMaps").Get<AzureMapsConfig>()!);
services.AddSingleton(configuration.GetSection("Styles").Get<StyleConfig>()!);
services.AddSingleton(configuration.GetSection("FeatureDownloader").Get<FeatureDownloaderConfig>()!);


services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser() // add more rules here if needed
        .Build();

    options.Filters.Add(new AuthorizeFilter(policy));
});

services.AddRazorPages()
        .AddMicrosoftIdentityUI();

var signalR = services.AddSignalR();
if (!builder.Environment.IsDevelopment())
{
    signalR.AddAzureSignalR();
}

services.AddSingleton(configuration.GetSection("Database").Get<DatabaseConfig>()!);
services.AddSingleton<FeatureStateRepository>();

services.AddSingleton<FeatureStatesStore>();

services.AddSingleton<AzureMapsTokenProvider>();
services.AddSingleton<FeatureDownloader>();
services.AddSingleton<FeaturesStore>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<FeatureStatesHub>("/featurestates");
app.MapControllers();
app.MapRazorPages();

app.Run();