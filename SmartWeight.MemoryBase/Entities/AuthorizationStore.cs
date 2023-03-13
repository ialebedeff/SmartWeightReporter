namespace SmartWeight.MemoryBase
{
    public class AuthorizationStore
    {
        public int Id { get; set; }
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool IsSucces { get; set; }
    }
}