using CsToml.Extensions.Configuration;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SimpleTomlValidation;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var httpClient = new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
};

try
{
    var toml = await httpClient.GetByteArrayAsync("appSettings.toml");
    var ms = new MemoryStream(toml);
    builder.Configuration.AddTomlStream(ms);
}
catch (Exception e)
{
    Console.WriteLine($"Failed to load appSettings.toml: {e}");
}

builder.Services.AddScoped(sp => httpClient);
await builder.Build().RunAsync();
