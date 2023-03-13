using System.Text.Json.Serialization;

namespace Entities
{
    public class Factory : EntityBase<int>
    {
        /// <summary>
        /// Название производства
        /// </summary>
        public string Title { get; set; } = null!;
        /// <summary>
        /// Вкл/Выкл обновлений для производства
        /// </summary>
        public bool IsUpdatesEnabled { get; set; }
        /// <summary>
        /// Учетная запись производства
        /// </summary>

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public User User { get; set; } = null!;
        /// <summary>
        /// Данные для подключения к БД
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public DatabaseConnection DatabaseConnection { get; set; } = null!;
        /// <summary>
        /// Заметки пользователя
        /// </summary>
        [JsonIgnore]
        public List<Note> Notes { get; set; } = new();
    }
}