using Communication;
using Communication.Configurator;
using Communication.Server;
using Entities;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using ReactiveUI;
using SmartWeight.Panel.Client.Pages.Login;
using SmartWeight.Updater.API;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;

namespace SmartWeight.Panel.Client.Components
{
    public class NoteAreaViewModel : ViewModelBase, IActivatableViewModel
    {
        public NoteAreaViewModel(
              ApplicationState applicationState
            , ISnackbar snackbar
            , IDialogService dialog
            , RestApiClients updaterApi
            , NavigationManager navigation
            , CommunicationService<ServerConfiguration> communicationService
            , DatabaseMessageFactory databaseMessageFactory) 
            : base(applicationState
                  , snackbar
                  , dialog
                  , updaterApi
                  , navigation
                  , communicationService
                  , databaseMessageFactory)
        {
            #region Инициализация команд
            CreateNoteCommand = ReactiveCommand.CreateFromTask(CreateNoteAsync);
            LoadNotesCommand = ReactiveCommand.CreateFromTask<int, IEnumerable<Note>?>(
                factoryId => updaterApi.Server.Note.GetNotesAsync(factoryId, 0, 20));
            #endregion
            #region Настраиваем IDisposable объекты на уничтожение вместе с ViewModel
            this.WhenActivated(disposables =>
            {
                LoadNotesCommand
                    .Execute(FactoryId)
                    .Subscribe(result =>
                    {
                        Notes = new ObservableCollection<Note>(result);
                    })
                    .DisposeWith(disposables);
                CreateNoteCommand
                    .Subscribe(OnCreateNoteRequested)
                    .DisposeWith(disposables);
            });
            #endregion
        }
        /// <summary>
        /// Команда для создания заметки
        /// </summary>
        public ReactiveCommand<Unit, OperationResult<Note>?> CreateNoteCommand { get; set; }
        /// <summary>
        /// Команда для загрузки заметок
        /// </summary>
        public ReactiveCommand<int, IEnumerable<Note>?> LoadNotesCommand { get; set; }
        /// <summary>
        /// Текст заметки
        /// </summary>
        public int FactoryId { get; set; }
        /// <summary>
        /// Текст заметки
        /// </summary>
        private string? _text;
        /// <summary>
        /// Текст заметки
        /// </summary>
        public string? Text
        {
            get => _text;
            set => this.RaiseAndSetIfChanged(ref _text, value);
        }
        /// <summary>
        /// Все заметки по текущему производству
        /// </summary>
        public ObservableCollection<Note> Notes { get; set; } = new();
        /// <summary>
        /// Активатор вью модели
        /// </summary>
        public ViewModelActivator Activator { get; set; }

        /// <summary>
        /// Создать заметку
        /// </summary>
        /// <returns></returns>
        private async Task<OperationResult<Note>?> CreateNoteAsync()
        {
            var request = new CreateNoteRequest(FactoryId, Text);
            var result = await ApiClients.Server.Note.CreateNoteAsync(request);

            return result;
        }
        /// <summary>
        /// Обработка результата создания заметки
        /// </summary>
        /// <param name="result"></param>
        public void OnCreateNoteRequested(OperationResult<Note>? result)
        {
            if (result is not null && result.Succeed)
            {
                Notes.Add(result.Result);
                ClearTextArea();
            }
        }
        /// <summary>
        /// Почистить Text area
        /// </summary>
        private void ClearTextArea() => Text = string.Empty;
    }
}
