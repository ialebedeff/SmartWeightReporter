using Entities;
using SmartWeight.MemoryBase;
using System;
using System.Threading.Tasks;

namespace SmartWeight.Updater.Pages.Updates.ViewModels
{
    public class UpdateInstallerProvider : UpdateProvider
    {
        private readonly ValidationProvider _validationProvider;
        private readonly BackupProvider _backupProvider;
        private readonly DownloadProvider _downloadProvider;
        private readonly PrepareProvider _prepareProvider;
        private readonly CopyProvider _copyProvider;
        private readonly CheckerProvider _checkerProvider;
        private readonly CompletionProvider _completionProvider;
        private readonly ApplicationBackupProvider _applicationBackupProvider;
        private readonly MigrateProvider _migrateProvider;
        private readonly UpdateHistoryStoreProvider _updateHistoryStoreProvider;
        public UpdateInstallerProvider(
            UpdateHistoryStoreProvider updateHistoryStoreProvider,
            BackupProvider backupProvider,
            CheckerProvider checkerProvider,
            CompletionProvider completionProvider,
            PrepareProvider prepareProvider,
            ApplicationBackupProvider applicationBackupProvider,
            ValidationProvider validationProvider,
            DownloadProvider downloadProvider,
            MigrateProvider migrateProvider,
            CopyProvider copyProvider)
        {
            _prepareProvider = prepareProvider;
            _downloadProvider = downloadProvider;
            _validationProvider = validationProvider;
            _copyProvider = copyProvider;
            _checkerProvider = checkerProvider;
            _completionProvider = completionProvider;
            _backupProvider = backupProvider;
            _applicationBackupProvider = applicationBackupProvider;
            _migrateProvider = migrateProvider;
            _updateHistoryStoreProvider = updateHistoryStoreProvider;
        }
        public override async Task ExecuteAsync(Build build, Action<ProgressState>? onProgressChanged = null)
        {
            if (build is null)
            {
                throw new UpdateException("Не выбрана сборка для установки", UpdateState.None);
            }

            await _prepareProvider.ExecuteAsync(build, onProgressChanged);
            await _checkerProvider.ExecuteAsync(build, onProgressChanged);
            await _downloadProvider.ExecuteAsync(build, onProgressChanged);
            await _validationProvider.ExecuteAsync(build, onProgressChanged);
            await _applicationBackupProvider.ExecuteAsync(build, onProgressChanged);
            await _backupProvider.ExecuteAsync(build, onProgressChanged);
            await _copyProvider.ExecuteAsync(build, onProgressChanged);
            await _completionProvider.ExecuteAsync(build, onProgressChanged);
            await _migrateProvider.ExecuteAsync(build, onProgressChanged);
        }
    }
}
