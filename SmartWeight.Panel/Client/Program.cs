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
using SmartWeight.Panel.Client.Pages.Tucks.ViewModels;
using SmartWeight.Panel.Client.Pages.Users.Collection;
using SmartWeight.Panel.Client.Pages.WeighingsData;
using SmartWeight.Panel.Client.Shared.ViewModels;
using SmartWeight.Updater.API;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddMudServices();
//builder.Services.AddSingleton<SmartWeightApi>(api => new SmartWeightApi("https://localhost:7274/", new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) }));

builder.Services.AddSingleton<RestApiClients>(api => new RestApiClients(new HttpClient()
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
}, null));

builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddSingleton<AuthenticationStateProvider, AuthStateProvider>();
builder.Services.AddSingleton<LoginViewModel>();
builder.Services.AddSingleton<FactoryCollectionViewModel>();
builder.Services.AddSingleton<CreateFactoryDialogViewModel>();
builder.Services.AddSingleton<SearchViewModel<Factory>>();
builder.Services.AddSingleton<SearchViewModel<User>>();
builder.Services.AddSingleton<NoteAreaViewModel>();
builder.Services.AddSingleton<FactoryViewModel>();
builder.Services.AddSingleton<ISnackbar, SnackbarService>();
builder.Services.AddSingleton<IDialogService, DialogService>();
builder.Services.AddSingleton<DatabaseMessageFactory>();
builder.Services.AddSingleton<DashboardViewModel>();
builder.Services.AddSingleton<ApplicationState>();
builder.Services.AddSingleton<UsersManagementViewModel>();
builder.Services.AddSingleton<FilterViewModel>();
builder.Services.AddTransient<FactoryCardViewModel>();
builder.Services.AddSingleton<FactorySelectionViewModel>();
builder.Services.AddSingleton<FilterableViewModel>();
builder.Services.AddSingleton<Filter>();
builder.Services.AddSingleton<TrucksViewModel>();
builder.Services.AddSingleton<TruckViewModel>();
builder.Services.AddSingleton<ChartsViewModel>();
builder.Services.AddSingleton<TruckDetailsViewModel>();
builder.Services.AddSingleton<AuthorizedDashboardViewModel>();
builder.Services.AddSingleton<ChartJS>();
builder.Services.AddSingleton<FinalWeighingsViewModel>();
builder.Services.AddSingleton<CommunicationService<ServerConfiguration>>(communication =>
{
    return new CommunicationService<ServerConfiguration>("https://localhost:7274/hub/smartweight", new ServerConfiguration());
});

await builder.Build().RunAsync();
