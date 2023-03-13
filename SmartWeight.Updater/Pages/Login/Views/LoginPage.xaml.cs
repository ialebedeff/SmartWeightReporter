using ReactiveUI;
using SmartWeight.Updater.Pages.Login.ViewModels;
using System.Windows.Input;

namespace SmartWeight.Updater.Pages.Login.Views;

/// <summary>
/// Логика взаимодействия для LoginPage.xaml
/// </summary>
public partial class LoginPage : ReactiveUserControl<LoginViewModel>
{
    public LoginPage()
    {
        InitializeComponent();

        ViewModel = ViewModelLocator.Instance.Resolve<LoginViewModel>();

        this.WhenActivated(disposableRegistration =>
        {
            // Notice we don't have to provide a converter, on WPF a global converter is
            // registered which knows how to convert a boolean into visibility.
            this.Bind(ViewModel,
                viewModel => viewModel.Login,
                view => view.login.Text);

            this.Bind(ViewModel,
                viewModel => viewModel.Password,
                view => view.password.Text);

            this.BindCommand(ViewModel,
                viewModel => viewModel.LoginCommand,
                view => view.loginButton);
        });
    }

    private void Card_MouseDown(object sender, MouseButtonEventArgs e)
        => App.Current.MainWindow.DragMove();
}
