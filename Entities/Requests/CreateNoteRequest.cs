namespace Entities
{
    /// <summary>
    /// Модель запроса для создания заметки
    /// </summary>
    public class CreateNoteRequest
    {
        public CreateNoteRequest(int factoryId, string text)
        {
            FactoryId = factoryId;
            Text = text;
        }
        public CreateNoteRequest() { }
        /// <summary>
        /// Идентификатор производства
        /// </summary>
        public int FactoryId { get; set; } = default(int)!;
        /// <summary>
        /// Текст заметки
        /// </summary>
        public string Text { get; set; } = null!;
    }
}