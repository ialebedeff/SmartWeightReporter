namespace SmartWeight.MemoryBase
{
    public class UpdateHistoryStoreProvider
    {
        private readonly DatabaseProvider _databaseProvider;
        public UpdateHistoryStoreProvider(DatabaseProvider databaseProvider)
        {
            _databaseProvider = databaseProvider;
        }

        public void AddUpdateHistory(UpdateHistoryStore updateHistoryStore)
        {
            _databaseProvider
                .UpdatesHistory
                .Insert(updateHistoryStore);
        }

        public IEnumerable<UpdateHistoryStore> GetUpdatesHistory()
        {
            return _databaseProvider
                .UpdatesHistory
                .Include(update => update.UpdateSteps)
                .FindAll();
        }
    }
}