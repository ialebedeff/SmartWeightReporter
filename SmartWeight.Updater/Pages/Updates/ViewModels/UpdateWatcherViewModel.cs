using DynamicData;
using Entities;
using MaterialDesignThemes.Wpf;
using ReactiveUI;
using SmartWeight.MemoryBase;
using SmartWeight.Updater.API;
using Splat;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace SmartWeight.Updater.Pages.Updates.ViewModels
{

    public class UpdateWatcherViewModel : ReactiveObject, IRoutableViewModel
    {
        private string? _status;
        private bool _isLoading;
        private bool _isValidating;
        private int _totalFiles;
        private int _downloadedFiles;
        private double _progress;

        private Build? _selectedBuild;

        private readonly SmartWeightApi _clientApi;
        private readonly UpdateInstallerProvider _updateInstallerProvider;
        public UpdateWatcherViewModel(
            UpdateInstallerProvider updateInstallerProvider,
            SmartWeightApi smartWeightApi)
        {
            _clientApi = smartWeightApi;
            _updateInstallerProvider = updateInstallerProvider;

            InstallCommand = ReactiveCommand.CreateFromTask(InstallUpdateAsync);
            InstallCommand.Subscribe(_ => OnInstallCompleted());
            InstallCommand.ThrownExceptions.Subscribe(e => OnInstallError(e));
            InstallCommand.IsExecuting.BindTo(this, view => view.IsLoading);

            //DownloadBuildCommand = ReactiveCommand.CreateFromTask(_ => DownloadUpdateAsync(SelectedBuild));
            //DownloadBuildCommand.Subscribe(_ => OnInstallUpdateSuccess());
            //DownloadBuildCommand.ThrownExceptions.Subscribe(e => OnInstallUpdateError(e));
            ////DownloadBuildCommand.IsExecuting.BindTo(this, view => view.IsLoading);

            //ValidateDownloadCommand = ReactiveCommand.CreateFromTask(_ => ValidateUpdateAsync(SelectedBuild));
            //ValidateDownloadCommand.Subscribe(_ => OnValidateComplete());
            //ValidateDownloadCommand.ThrownExceptions.Subscribe(e => OnInstallUpdateError(e));
            ////ValidateDownloadCommand.IsExecuting.BindTo(this, view => IsValidating);

            //InstallBuildCommand = ReactiveCommand.CreateCombined(new ReactiveCommand<Unit, Unit>[]
            //{
            //    DownloadBuildCommand, ValidateDownloadCommand
            //});

            //InstallBuildCommand.IsExecuting.BindTo(this, view => view.IsLoading);
            //InstallBuildCommand.ThrownExceptions.Subscribe(e => OnInstallUpdateError(e));

            _ = GetAvailableBuildsAsync();
        }
        public ReactiveCommand<Unit, Unit> InstallCommand { get; set; }
        public ObservableCollection<Build> AvailableBuilds { get; set; } = new();
        public Build? SelectedBuild
        {
            get => _selectedBuild;
            set => this.RaiseAndSetIfChanged(ref _selectedBuild, value);
        }
        public double Progress
        {
            get => _progress;
            set => this.RaiseAndSetIfChanged(ref _progress, value);
        }
        public string Status
        {
            get => _status;
            set => this.RaiseAndSetIfChanged(ref _status, value);
        }
        public int TotalFiles
        {
            get => _totalFiles;
            set => this.RaiseAndSetIfChanged(ref _totalFiles, value);
        }
        public int DownloadedFiles
        {
            get => _downloadedFiles;
            set => this.RaiseAndSetIfChanged(ref _downloadedFiles, value);
        }
        public bool IsLoading
        {
            get => _isLoading;
            set => this.RaiseAndSetIfChanged(ref _isLoading, value);
        }
        public bool IsValidating
        {
            get => _isValidating;
            set => this.RaiseAndSetIfChanged(ref _isValidating, value);
        }

        public string? UrlPathSegment { get; set; }
        public IScreen HostScreen { get; set; }

        private async Task GetAvailableBuildsAsync()
        {
            var availableBuilds = await _clientApi.Server.Update.GetBuildsAsync();

            if (availableBuilds is not null)
            {
                AvailableBuilds.AddRange(availableBuilds);
            }
        }

        public Task InstallUpdateAsync()
        {
            Status = "Запуск...";
            return _updateInstallerProvider.ExecuteAsync(SelectedBuild, OnProgressChanged);
        }

        private void OnProgressChanged(ProgressState progressState)
        {
            Status = progressState.Status;
            TotalFiles = progressState.TotalFiles;
            DownloadedFiles = progressState.TotalDownloadedFiles;

            if (TotalFiles is not 0)
            {
                Progress = (double)DownloadedFiles / TotalFiles;
            }
        }

        private void OnInstallError(Exception exception)
        {
            // ToDo: Нужно будет обработать некоторые сценарии ошибок
            // Пока что при ошибке в любом шаге обновления, обновление сбрасывается
            if (exception is UpdateException updateException)
            {
                switch (updateException.UpdateState)
                {
                    case UpdateState.Download: break;
                    case UpdateState.Validate: break;
                    case UpdateState.Check: break;
                    case UpdateState.DatabaseBackup: break;
                    case UpdateState.Prepare: break;
                    case UpdateState.Copy: break;
                }
            }
            else
            {
                MessageBox.Show(exception.Message);
            }
        }

        public void OnInstallCompleted()
        {
            MessageBox.Show("Установка успешно выполнена");
        }
    }
}
