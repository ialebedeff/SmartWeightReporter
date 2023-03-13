namespace Entities
{
    /// <summary>
    /// Модель заметки
    /// </summary>
    public class Note : EntityBase<int>
    { 
        /// <summary>
        /// Пользователь оставивший заметку
        /// </summary>
        public User User { get; set; } = null!;
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// Производство, на котором висит заметка
        /// </summary>
        public Factory Factory { get; set; } = null!;
        /// <summary>
        /// Идентификатор производства
        /// </summary>
        public int FactoryId { get; set; }
        /// <summary>
        /// Текст заметки
        /// </summary>
        public string Text { get; set; } = null!;
        /// <summary>
        /// Дата создания заметки
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.UtcNow;
    }
}