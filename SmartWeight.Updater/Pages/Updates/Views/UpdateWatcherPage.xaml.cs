using ReactiveUI;
using SmartWeight.Updater.Pages.Updates.ViewModels;
using System.Windows.Input;

namespace SmartWeight.Updater.Pages.Updates.Views;

/// <summary>
/// Логика взаимодействия для UpdateWatcherPage.xaml
/// </summary>
public partial class UpdateWatcherPage : ReactiveUserControl<UpdateWatcherViewModel>
{
    public UpdateWatcherPage()
    {
        InitializeComponent();

        ViewModel = ViewModelLocator.Instance.Resolve<UpdateWatcherViewModel>();

        this.WhenActivated(disposableRegistration =>
        {
            // Notice we don't have to provide a converter, on WPF a global converter is
            // registered which knows how to convert a boolean into visibility.
            this.OneWayBind(ViewModel,
                viewModel => viewModel.AvailableBuilds,
                view => view.builds.ItemsSource);

            // Биндинг выбранного билда для установки
            this.Bind(ViewModel,
               viewModel => viewModel.SelectedBuild,
               view => view.builds.SelectedItem);

            // Биндинг статуса обновлений
            this.Bind(ViewModel,
              viewModel => viewModel.Status,
              view => view.status.Text);

            // Биндинг значения прогресс бара
            this.OneWayBind(ViewModel,
              viewModel => viewModel.Progress,
              view => view.progressBar.Value);

            // Биндинг команды на начало установки
            this.BindCommand(ViewModel,
                viewModel => viewModel.InstallCommand,
                view => view.installButton);

            // Биндинг панели инофрмации
            // по обновлению к статусу загрузки
            this.OneWayBind(ViewModel,
                viewModel => viewModel.IsLoading,
                view => view.loadingInfo.Visibility);
        });
    }

    private void Card_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            App.Current.MainWindow.DragMove();
        }
    } 
}
