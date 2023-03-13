using Entities;

namespace SmartWeight.Updater.API
{
    public class NoteApi : HttpClientBase
    {
        public NoteApi(HttpClient httpClient) : base(httpClient)
        {
        }
        /// <summary>
        /// Получить заметки по объекту
        /// </summary>
        /// <param name="factoryId"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public Task<IEnumerable<Note>?> GetNotesAsync(int factoryId, int skip, int take)
            => GetAsync<IEnumerable<Note>>($"api/note/GetNotes?factoryId={factoryId}&skip={skip}&take={take}");
        /// <summary>
        /// Создать заметку
        /// </summary>
        /// <param name="createNoteRequest"></param>
        /// <returns></returns>
        public Task<OperationResult<Note>?> CreateNoteAsync(CreateNoteRequest createNoteRequest)
            => PostAsync<CreateNoteRequest, OperationResult<Note>>("api/note/createNote", createNoteRequest);
    }
}