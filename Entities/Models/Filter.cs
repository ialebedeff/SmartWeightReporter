namespace Entities
{
    /// <summary>
    /// Фильтр
    /// </summary>
    public class Filter
    {
        /// <summary>
        /// Номер машины
        /// </summary>
        public string? CarNumber { get; set; }
        /// <summary>
        /// Дата с которой нужно 
        /// начать поиск
        /// </summary>
        public DateTime? From { get; set; }
        /// <summary>
        /// Дата на которой нужно 
        /// завершить поиск
        /// </summary>
        public DateTime? To { get; set; }
        /// <summary>
        /// Материал
        /// </summary>
        public string? Material { get; set; }
        /// <summary>
        /// Поставщик
        /// </summary>
        public string? Shipper { get; set; }
        /// <summary>
        /// Контрагент
        /// </summary>
        public string? Contragent { get; set; }
    }
}