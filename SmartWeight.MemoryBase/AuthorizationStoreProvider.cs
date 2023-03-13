namespace SmartWeight.MemoryBase
{
    public class AuthorizationStoreProvider
    {
        private readonly DatabaseProvider _databaseProvider;
        public AuthorizationStoreProvider(DatabaseProvider databaseProvider) 
        {
            _databaseProvider = databaseProvider;
        }

        public void AddAuthorization(string login, string password, bool isSuccess)
        {
            _databaseProvider
                .AuthorizationStores
                .Insert(new AuthorizationStore() { IsSucces = isSuccess, Login = login, Password = password });
        }

        public AuthorizationStore GetLastSuccessAuthorization()
        {
            return _databaseProvider
                .AuthorizationStores
                .Query()
                .OrderByDescending(auth => auth.Id)
                .Where(auth => auth.IsSucces)
                .FirstOrDefault();
        }
    }
}