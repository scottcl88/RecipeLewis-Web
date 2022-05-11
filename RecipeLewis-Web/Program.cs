using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using RecipeLewis_Web;
using BlazorApp.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var services = builder.Services;

services
    .AddScoped<IAuthenticationService, AuthenticationService>()
    .AddScoped<IUserService, UserService>()
    .AddScoped<IHttpService, HttpService>()
    .AddScoped<ILocalStorageService, LocalStorageService>();

//services.AddApiAuthorization();
//services.AddAuthorizationCore();

//services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

//services.AddOidcAuthentication(options =>
//{
//    // Configure your authentication provider options here.
//    // For more information, see https://aka.ms/blazor-standalone-auth
//    builder.Configuration.Bind("Local", options.ProviderOptions);
//});

// configure http client
services.AddScoped(x => {
    var apiUrl = new Uri(builder.Configuration["apiUrl"]);

    // use fake backend if "fakeBackend" is "true" in appsettings.json
    //if (builder.Configuration["fakeBackend"] == "true")
    //    return new HttpClient(new FakeBackendHandler()) { BaseAddress = apiUrl };

    return new HttpClient() { BaseAddress = apiUrl };
});

var authenticationService = builder.Build().Services.GetRequiredService<IAuthenticationService>();
await authenticationService.Initialize();

await builder.Build().RunAsync();
