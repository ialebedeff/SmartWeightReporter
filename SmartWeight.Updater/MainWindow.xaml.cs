using ReactiveUI;
using SmartWeight.Updater.Pages.Main.ViewModels;
using System.Reactive.Disposables;

namespace SmartWeight.Updater;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : ReactiveWindow<MainViewModel>
{
    public MainWindow()
    {
        InitializeComponent();

        ViewModel = ViewModelLocator.Instance.Resolve<MainViewModel>();

        this.WhenActivated(disposableRegistration =>
        {
            // Notice we don't have to provide a converter, on WPF a global converter is
            // registered which knows how to convert a boolean into visibility.

            // Бинд команды на выход из аккаунта
            this.BindCommand(ViewModel,
                viewModel => viewModel.SignOutCommand,
                view => view.signOutButton);

            // Бинд видимости бокового меню к статусу авторизации
            this.OneWayBind(ViewModel,
                viewModel => viewModel.IsAuthorized,
                view => view.DrawerHost.Visibility);

            // Бинд роутера
            this.OneWayBind(ViewModel, x => x.Router, x => x.RoutedViewHost.Router)
                .DisposeWith(disposableRegistration);
        });
    }
}
