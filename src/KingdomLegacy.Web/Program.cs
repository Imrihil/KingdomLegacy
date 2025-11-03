using Blazored.LocalStorage;
using KingdomLegacy.Domain;
using KingdomLegacy.Web;
using KingdomLegacy.Web.Configuration;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
    .AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
    .AddScoped<Game>()
    .AddScoped<Resources>()
    .AddScoped<SavedGameData>()
    .AddBlazorBootstrap()
    .AddBlazoredLocalStorage()
    .AddBlazoredLocalStorage(config => config.JsonSerializerOptions.WriteIndented = true);

await builder.Build().RunAsync();
