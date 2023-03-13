using Communication;
using Communication.Configurator;
using Communication.Server;
using Entities;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;
using SmartWeight.Admin.Client.Pages.Factories;
using SmartWeight.Panel.Client;
using SmartWeight.Panel.Client.Components;
using SmartWeight.Panel.Client.Dialogs;
using SmartWeight.Panel.Client.Pages.Charts;
using SmartWeight.Panel.Client.Pages.Dashboard;
using SmartWeight.Panel.Client.Pages.Factories.FactorySelection;
using SmartWeight.Panel.Client.Pages.Factories.FactoryView;
using SmartWeight.Panel.Client.Pages.Login;
using SmartWeight.Panel.Client.Pages.Users.Collection;
using SmartWeight.Panel.Client.Pages.WeighingsData;
using SmartWeight.Panel.Client.Shared.ViewModels;
using SmartWeight.Updater.API;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddMudServices();
//builder.Services.AddSingleton<SmartWeightApi>(api => new SmartWeightApi("https://localhost:7274/", new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) }));

builder.Services.AddScoped<RestApiClients>(api => new RestApiClients(new HttpClient()
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
}, null));
builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
builder.Services.AddScoped<LoginViewModel>();
builder.Services.AddScoped<FactoryCollectionViewModel>();
builder.Services.AddScoped<CreateFactoryDialogViewModel>();
builder.Services.AddScoped<SearchViewModel<Factory>>();
builder.Services.AddScoped<SearchViewModel<User>>();
builder.Services.AddScoped<NoteAreaViewModel>();
builder.Services.AddScoped<FactoryViewModel>();
builder.Services.AddScoped<ISnackbar, SnackbarService>();
builder.Services.AddScoped<IDialogService, DialogService>();
builder.Services.AddScoped<DatabaseMessageFactory>();
builder.Services.AddScoped<DashboardViewModel>();
builder.Services.AddScoped<ApplicationState>();
builder.Services.AddScoped<UsersManagementViewModel>();
builder.Services.AddScoped<FilterViewModel>();
builder.Services.AddTransient<FactoryCardViewModel>();
builder.Services.AddScoped<FactorySelectionViewModel>();
builder.Services.AddScoped<FilterableViewModel>();
builder.Services.AddScoped<Filter>();
builder.Services.AddScoped<ChartsViewModel>();
builder.Services.AddScoped<AuthorizedDashboardViewModel>();
builder.Services.AddScoped<ChartJS>();
builder.Services.AddScoped<FinalWeighingsViewModel>();
builder.Services.AddScoped<CommunicationService<ServerConfiguration>>(communication =>
{
    return new CommunicationService<ServerConfiguration>("https://localhost:7274/hub/smartweight", new ServerConfiguration());
});

await builder.Build().RunAsync();
