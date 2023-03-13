using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using SmartWeight.MemoryBase;
using SmartWeight.Updater.API;
using SmartWeight.Updater.Pages.Login.ViewModels;
using SmartWeight.Updater.Pages.Main.ViewModels;
using SmartWeight.Updater.Pages.Updates.ViewModels;
using Splat;
using System;
using System.Net.Http;
using System.Reflection;
using System.Windows;

namespace SmartWeight.Updater
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App() {
            // A helper method that will register all classes that derive off IViewFor 
            // into our dependency injection container. ReactiveUI uses Splat for it's 
            // dependency injection by default, but you can override this if you like.
            Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetCallingAssembly());
        }
    }

    public class ViewModelLocator
    {
        private static Lazy<ViewModelLocator> _instance 
            = new Lazy<ViewModelLocator>(new ViewModelLocator());
        public static ViewModelLocator Instance => _instance.Value;
        private readonly IServiceProvider _serviceProvider;
        public ViewModelLocator()
        {
            ServiceCollection services = new ServiceCollection();
            
            // Регистрация ViewModel и сервисов
            services.AddSingleton<HttpClient>(options => new HttpClient() { BaseAddress = new Uri("https://localhost:7274/") });
            services.AddSingleton<SmartWeightApi>(options 
                => new SmartWeightApi(new HttpClient(new HttpClientHandler() 
                {
                    CookieContainer = new System.Net.CookieContainer(), 
                    UseCookies = true
                }) 
                {
                    BaseAddress = new Uri("http://localhost:5080/") 
                }, new HttpClient() { 
                    BaseAddress = new Uri("http://localhost:5126/")
                }));

            services.AddSingleton<MainViewModel>();
            services.AddSingleton<UpdateWatcherViewModel>();
            services.AddSingleton<UpdateInstallerViewModel>();
            services.AddSingleton<AuthorizationStoreProvider>();
            services.AddSingleton<UpdateHistoryStoreProvider>();
            services.AddSingleton<DatabaseProvider>();
            services.AddSingleton<DownloadProvider>();
            services.AddSingleton<ValidationProvider>();
            services.AddSingleton<PrepareProvider>();
            services.AddSingleton<CheckerProvider>();
            services.AddSingleton<CompletionProvider>();
            services.AddSingleton<CopyProvider>();
            services.AddSingleton<UpdateInstallerProvider>();
            services.AddSingleton<ApplicationBackupProvider>();
            services.AddSingleton<MigrateProvider>();
            services.AddSingleton<BackupProvider>();
            services.AddSingleton<LoginViewModel>();

            _serviceProvider = services.BuildServiceProvider();
        }

        public TService Resolve<TService>() 
            => _serviceProvider.GetRequiredService<TService>();
    }
}
