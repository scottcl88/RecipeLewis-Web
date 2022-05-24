using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using RecipeLewis_Web;
using BlazorApp.Services;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var services = builder.Services;

services.AddMudServices();

services.AddAutoMapper(typeof(OrganizationProfile));

services
    .AddScoped<IAuthenticationService, AuthenticationService>()
    .AddScoped<IUserService, UserService>()
    .AddScoped<IRecipeService, RecipeService>()
    .AddScoped<IDocumentService, DocumentService>()
    .AddScoped<IHttpService, HttpService>()
    .AddScoped<ILocalStorageService, LocalStorageService>()
    .AddSingleton(new AuthUserSingleton());

services.AddScoped(x => {
    var apiUrl = new Uri(builder.Configuration["apiUrl"]);
    return new HttpClient() { BaseAddress = apiUrl };
});

var authenticationService = builder.Build().Services.GetRequiredService<IAuthenticationService>();
await authenticationService.Initialize();

await builder.Build().RunAsync();
