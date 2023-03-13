using ReactiveUI;

using SmartWeight.Updater.API;
using SmartWeight.Updater.Pages.Login.ViewModels;
using SmartWeight.Updater.Pages.Updates.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SmartWeight.Updater.Pages.Main.ViewModels
{
    public class MainViewModel : ReactiveObject, IScreen
    {
        private RoutingState _router = new RoutingState();
        private readonly SmartWeightApi _clientApi;
        private bool _isAuthorized;
        public MainViewModel(SmartWeightApi smartWeightApi)
        {
            _clientApi = smartWeightApi;
            _clientApi.Server.Authorization.AuthorizationStateChanged += Authorization_AuthorizationStateChanged;

            Router.Navigate.Execute(ViewModelLocator.Instance.Resolve<LoginViewModel>());
            SignOutCommand = ReactiveCommand.CreateFromTask(SignOutAsync);
        }
        public ReactiveCommand<Unit, Unit> SignOutCommand { get; set; }
      
        public bool IsAuthorized
        { 
            get => _isAuthorized;
            set => this.RaiseAndSetIfChanged(ref _isAuthorized, value);
        }
        public RoutingState Router
        {
            get => _router;
            set => this.RaiseAndSetIfChanged(ref _router, value);
        }

        public Task SignOutAsync()
            => _clientApi.Server.Authorization.SignOutAsync();
        private void Authorization_AuthorizationStateChanged(object? sender, AuthorizationState e)
        {
            IsAuthorized = e.IsAuthorized;

            if (e.IsAuthorized) Router.Navigate.Execute(ViewModelLocator.Instance.Resolve<UpdateWatcherViewModel>());
            else Router.Navigate.Execute(ViewModelLocator.Instance.Resolve<LoginViewModel>());
        }
    }
}
