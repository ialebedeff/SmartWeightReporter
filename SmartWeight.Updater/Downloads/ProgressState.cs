namespace SmartWeight.Updater.Pages.Updates.ViewModels
{
    public class DownloadState
    {
        public bool IsLoading { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsFaulted { get; set; }

        public string? ErrorMessage { get; set; }

        public static DownloadState Started() => new DownloadState() { IsLoading = true };
        public static DownloadState Faulted(string? errorMessage = null) => new DownloadState()
        {
            IsFaulted = true,
            ErrorMessage = errorMessage
        };
        public static DownloadState Completed() => new DownloadState() { IsCompleted = true };
    }
    public class ProgressState
    {
        public ProgressState(string? fileName, string status)
        {
            CurrentFileName = fileName ?? string.Empty;
            Status = status;
        }
        public ProgressState(
            int totalFiles,
            int totalDownloadedFiles,
            string currentFilename, string status)
        {
            TotalFiles = totalFiles;
            TotalDownloadedFiles = totalDownloadedFiles;
            CurrentFileName = currentFilename;
            Status = status;
        }

        public int TotalFiles { get; set; }
        public int TotalDownloadedFiles { get; set; }
        public string CurrentFileName { get; set; }
        public string Status { get; set; }
    }
}
