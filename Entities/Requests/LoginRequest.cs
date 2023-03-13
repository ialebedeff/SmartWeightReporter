namespace Entities
{
    /// <summary>
    /// Модель запроса 
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Логин / Юзернейм
        /// </summary>
        public string Login { get; set; } = null!;
        /// <summary>
        /// Пароль
        /// </summary>
        public string Password { get; set; } = null!;
    }
}