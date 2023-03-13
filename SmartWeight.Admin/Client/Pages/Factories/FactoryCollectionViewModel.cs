using SmartWeight.Admin.Client.Dialogs;
using SmartWeight.Admin.Client.Pages.Login;

namespace SmartWeight.Admin.Client.Pages.Factories
{
    public class FactoryCollectionViewModel : ViewModelBase
    {


        public Task OpenCreateFactoryDialogAsync()
            => Dialog.ShowAsync<CreateFactoryDialog>("", new MudBlazor.DialogOptions() { MaxWidth = MudBlazor.MaxWidth.Medium });
    }
}
