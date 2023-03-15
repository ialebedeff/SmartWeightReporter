using Communication.Configurator;
using Dapper;
using Entities;
using Entities.Database;
using Microsoft.AspNetCore.SignalR.Client;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;
using System.Diagnostics;

namespace Communication.Client
{
    public class DatabaseQueryExecutor
    {
        private static Lazy<DatabaseQueryExecutor> _instance = new Lazy<DatabaseQueryExecutor>(new DatabaseQueryExecutor());
        public static DatabaseQueryExecutor Instance => _instance.Value;
        private MySqlConnection? _connection;
        public async Task<IEnumerable<TOutput>> ExecuteDatabaseCommandAsync<TOutput>(DatabaseCommand databaseCommand)
        {
            if (await CreateConnectionIfNotExistsAsync(databaseCommand.Connection))
            {
                return _connection.Query<TOutput>(databaseCommand.Command);
            }

            return Enumerable.Empty<TOutput>();
        }

        public async Task<IEnumerable<Truck>> ExecuteTrucksCommandAsync(DatabaseCommand databaseCommand)
        {
            if (await CreateConnectionIfNotExistsAsync(databaseCommand.Connection))
            {
                return _connection.Query<Truck, Driver, Truck>(databaseCommand.Command, (truck, driver) =>
                {
                    truck.Driver = driver;
                    return truck; ;
                });
            }

            return Enumerable.Empty<Truck>();
        }

        public Task<bool> CreateConnectionIfNotExistsAsync(DatabaseConnection databaseConnection)
        {
            if (_connection is not null && _connection.State is ConnectionState.Open)
                return Task.FromResult(true);

            if (_connection is not null && _connection.State is ConnectionState.Closed)
                return _connection.OpenAsync().ContinueWith(task => task.IsCompletedSuccessfully);

            var connectionBuild = new MySqlConnectionStringBuilder();

            connectionBuild.UserID = databaseConnection.User;
            connectionBuild.Server = databaseConnection.Server;
            connectionBuild.Password = databaseConnection.Password;
            connectionBuild.Database = databaseConnection.Database;

            var connectionString = connectionBuild.ToString();

            _connection = new MySqlConnection(connectionString);

            return _connection
                .OpenAsync()
                .ContinueWith(task => task.IsCompletedSuccessfully);
        }
    }
    public partial class DatabaseExecutor : MessageExecutor<DatabaseCommand, List<Dictionary<string, object>>>
    {
        private MySqlConnection? sqlConnection;
        public DatabaseExecutor(HubConnection hubConnection, string methodName) : base(hubConnection, methodName)
        {
        }
        public override Task<List<Dictionary<string, object>>> ExecuteAsync(Message<DatabaseCommand> databaseCommand)
        {
            return ExecuteCommandAsync(databaseCommand.Data);
        }
    }

    public class TruckDatabaseExecutor : DatabaseMessageExecutor<Truck>
    {
        public TruckDatabaseExecutor(HubConnection hubConnection, string responseUrl) : base(hubConnection, responseUrl)
        {
        }

        public override Task<IEnumerable<Truck>> ExecuteCommandAsync(DatabaseCommand databaseCommand)
            => DatabaseQueryExecutor.Instance.ExecuteTrucksCommandAsync(databaseCommand);
    }

    public class WeighingsDatabaseExecutor : DatabaseMessageExecutor<Weighings>
    {
        public WeighingsDatabaseExecutor(HubConnection hubConnection, string responseUrl) : base(hubConnection, responseUrl)
        {
        }

        public override Task<IEnumerable<Weighings>> ExecuteCommandAsync(DatabaseCommand databaseCommand)
            => DatabaseQueryExecutor.Instance.ExecuteDatabaseCommandAsync<Weighings>(databaseCommand);
    }

    public partial class DatabaseExecutor
    {
        public async Task<IEnumerable<TOutput>> ExecuteCommandAsync<TOutput>(DatabaseCommand databaseCommand)
        {
            if (await CreateConnectionIfNotExistsAsync(databaseCommand.Connection))
            {
                return sqlConnection.Query<TOutput>(databaseCommand.Command);
            }

            return Enumerable.Empty<TOutput>();
        }
        public async Task<List<Dictionary<string, object>>> ExecuteCommandAsync(DatabaseCommand databaseCommand)
        {
            if (await CreateConnectionIfNotExistsAsync(databaseCommand.Connection))
            {
                using (var command = CreateMySqlCommand(databaseCommand))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    try
                    {
                        Debug.WriteLine(command.CommandText);
                        return await ReadResultAsync(reader);
                    }
                    catch (Exception ex) { Debug.WriteLine(ex.Message); }
                }
            }

            return new List<Dictionary<string, object>>();
        }
        private MySqlCommand CreateMySqlCommand(DatabaseCommand databaseCommand)
        {
            return new MySqlCommand(databaseCommand.Command, sqlConnection);
        }
        public async Task<bool> CreateConnectionIfNotExistsAsync(DatabaseConnection connection)
        {
            if (sqlConnection is not null &&
                sqlConnection.State is ConnectionState.Open)
            {
                return true;
            }

            MySqlConnectionStringBuilder mySqlConnectionStringBuilder = new MySqlConnectionStringBuilder();

            mySqlConnectionStringBuilder.UserID = connection.User;
            mySqlConnectionStringBuilder.Password = connection.Password;
            mySqlConnectionStringBuilder.Database = connection.Database;
            mySqlConnectionStringBuilder.Server = connection.Server;

            string connectionString = mySqlConnectionStringBuilder.ConnectionString;

            sqlConnection = new MySqlConnection(connectionString);

            try
            {
                await sqlConnection.OpenAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при подключение к БД: {ex.Message}");
                return false;
            }
        }
        private async Task<List<Dictionary<string, object>>> ReadResultAsync(DbDataReader reader)
        {
            var values = new List<Dictionary<string, object>>();

            while (await reader.ReadAsync())
            {
                values.Add(new Dictionary<string, object>());

                for (int i = 0; i < reader.FieldCount; i += 1)
                {
                    if (!values.Last().ContainsKey(reader.GetName(i)))
                    {
                        values.Last().Add(reader.GetName(i),
                            reader.GetValue(i));
                    }
                    else
                    {
                        values.Last()[reader.GetName(i)] = reader.GetValue(i);
                    }
                }
            }

            return values;
        }
    }
}