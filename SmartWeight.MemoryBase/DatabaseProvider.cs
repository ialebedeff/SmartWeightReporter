using LiteDB;

namespace SmartWeight.MemoryBase
{
    public class DatabaseProvider
    {
        private readonly LiteDatabase _database;
        public DatabaseProvider() {
            _database = new LiteDatabase(@"Database.db");
        }

        public ILiteCollection<AuthorizationStore> AuthorizationStores => _database.GetCollection<AuthorizationStore>();
        public ILiteCollection<UpdateHistoryStore> UpdatesHistory => _database.GetCollection<UpdateHistoryStore>();
    }
}