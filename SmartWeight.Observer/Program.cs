using Communication;
using Communication.Client;
using SmartWeight.Updater.API;

var httpClientBuilder = new HttpClientBuilder();
var httpClient = httpClientBuilder.Build("https://localhost:7274", new System.Net.CookieContainer());
var restApiClient = new SmartWeightApi(httpClient, null);
await restApiClient.Server.Authorization.SignInAsync("test123", "Oliver15243@");

var factories = await restApiClient.Server.Factory.GetCurrentUserFactoriesAsync();
var factory = factories.Last();

var communicationService = new CommunicationService<ClientConfiguration>(
    "https://localhost:7274/hub/smartweight",
    new ClientConfiguration(), httpClientBuilder.CookieContainer);
await communicationService.StartAsync();
await communicationService.RegisterAsClientFactoryAsync(factory);

Console.ReadLine();