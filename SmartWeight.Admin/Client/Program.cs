using Blazored.LocalStorage;
using ClientApi;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using SmartWeight.Admin.Client;
using SmartWeight.Admin.Client.Pages.Login;
using System.Net;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var client = new HttpClient() { BaseAddress = new Uri("https://localhost:7170/") };

builder.Services.AddBlazoredLocalStorage();   // local storage
builder.Services.AddBlazoredLocalStorage(config => config.JsonSerializerOptions.WriteIndented = true);  // local storage


builder.Services.AddSingleton<HttpClient>(client);
builder.Services.AddSingleton<ApplicationState>();
builder.Services.AddSingleton<Uri>(new Uri("https://localhost:7170/"));
builder.Services.AddSingleton<SmartWeightApi>();
builder.Services.AddMudServices();

await builder.Build().RunAsync();
