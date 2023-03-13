namespace Entities
{
    /// <summary>
    /// Модель запроса на создание 
    /// учетной записи пользователя
    /// </summary>
    public class CreateFactoryUserRequest
    {
        /// <summary>
        /// Произвосдтва пользователя
        /// </summary>
        public IEnumerable<FactoryDto> Factories { get; set; } = null!;
        /// <summary>
        /// Юзернейм пользователя
        /// </summary>
        public string UserName { get; set; } = null!;
        /// <summary>
        /// Почтовый адрес пользователя
        /// </summary>
        public string Email { get; set; } = null!;
        /// <summary>
        /// Пароль пользователя
        /// </summary>
        public string Password { get; set; } = null!;
        /// <summary>
        /// Обновления пользователя
        /// </summary>
        public bool IsUpdateEnabled { get; set; }
    }
}