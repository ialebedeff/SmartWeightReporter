namespace Entities
{
    public class DatabaseCommand 
    {
        public string Command { get; set; } = null!;
        public DatabaseConnection Connection { get; set; } = null!;
        public DatabaseCommand() { }
        public DatabaseCommand(DatabaseConnection databaseConnection) 
            => this.Connection = databaseConnection;
        public DatabaseCommand(Factory factory) : this(factory.DatabaseConnection) { }
    }
}