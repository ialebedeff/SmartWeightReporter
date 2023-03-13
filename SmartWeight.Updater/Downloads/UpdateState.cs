namespace SmartWeight.Updater.Pages.Updates.ViewModels
{
    public enum UpdateState
    { 
        None,
        Check,
        Prepare,
        Download,
        Validate,
        DatabaseBackup,
        ApplicationBackup,
        Copy,
        Completion,
        Migrate
    }
}
