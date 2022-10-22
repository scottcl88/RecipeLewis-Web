using BlazorApp.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using RecipeLewis_Web;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var env = builder.HostEnvironment;

//builder.Configuration
// .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
//.AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var services = builder.Services;

services.AddMudServices();

services.AddAutoMapper(typeof(OrganizationProfile));

services
    .AddScoped<MenuService>()
    .AddScoped<IAuthenticationService, AuthenticationService>()
    .AddScoped<IUserService, UserService>()
    .AddScoped<IRecipeService, RecipeService>()
    .AddScoped<IDocumentService, DocumentService>()
    .AddScoped<IHttpService, HttpService>()
    .AddScoped<ILocalStorageService, LocalStorageService>()
    .AddSingleton(new AuthUserSingleton());

services.AddScoped(x => {
    if (env.IsDevelopment())
    {
        var apiUrl = new Uri("https://localhost:7039/api/");
        return new HttpClient() { BaseAddress = apiUrl };
    }
    else
    {
        var apiUrl = new Uri("https://recipelewis-api.azurewebsites.net/api/");
        return new HttpClient() { BaseAddress = apiUrl };
    }
});

var authenticationService = builder.Build().Services.GetRequiredService<IAuthenticationService>();
await authenticationService.Initialize();

await builder.Build().RunAsync();