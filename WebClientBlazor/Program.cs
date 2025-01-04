using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored.LocalStorage;
using WebClientBlazor.Helpers;
using WebClientBlazor.Services;
using WebClientBlazor;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Register HttpClient for API communication
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7152/") // Replace with your actual API base URL
});

// Add logging
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Add Blazored.LocalStorage for token storage
builder.Services.AddBlazoredLocalStorage();

// Register the custom authentication state provider
builder.Services.AddScoped<TokenAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<TokenAuthenticationStateProvider>());
builder.Services.AddAuthorizationCore();

// Add your UserService
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<BookService>();

await builder.Build().RunAsync();
