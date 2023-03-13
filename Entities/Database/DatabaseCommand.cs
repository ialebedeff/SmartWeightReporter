namespace Entities
{
    public class DatabaseCommand 
    {
        public string Command { get; set; } = null!;
        public DatabaseConnection Connection { get; set; } = null!;
    }
}