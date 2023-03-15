using Communication;
using Communication.Client;
using Dapper.FluentMap;
using Entities.Database;
using EntitiesMapper;
using SmartWeight.Updater.API;


FluentMapper.Initialize(config =>
{
    config.AddMap(new WeighingsProfile());
    config.AddMap(new BrandProfile());
    config.AddMap(new CarTypesProfile());
    config.AddMap(new ModelProfile());
    config.AddMap(new VehicleProfile());
    config.AddMap(new DriverProfile());
    config.AddMap(new TruckProfile());
});

var httpClientBuilder = new HttpClientBuilder();
var httpClient = httpClientBuilder.Build("https://localhost:7274", new System.Net.CookieContainer());
var restApiClient = new RestApiClients(httpClient, null);
await restApiClient.Server.Authorization.SignInAsync("test123", "Oliver15243@");

var factories = await restApiClient.Server.Factory.GetCurrentUserFactoriesAsync();
var factory = factories.Last();

var communicationService = new CommunicationService<ClientConfiguration>(
    "https://localhost:7274/hub/smartweight",
    new ClientConfiguration(), httpClientBuilder.CookieContainer);

await communicationService.StartAsync();
await communicationService.RegisterAsClientFactoryAsync(factory);

Console.ReadLine();