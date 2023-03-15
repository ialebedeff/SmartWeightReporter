using Communication.Configurator;
using Entities;
using Microsoft.AspNetCore.SignalR.Client;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;
using System.Diagnostics;

namespace Communication.Client
{
    public partial class DatabaseMessageExecutor : MessageExecutor<DatabaseCommand, List<Dictionary<string, object>>>
    {
        private MySqlConnection? sqlConnection;
        public DatabaseMessageExecutor(HubConnection hubConnection, string methodName) : base(hubConnection, methodName) 
        {
        }
        public override Task<List<Dictionary<string, object>>> ExecuteAsync(Message<DatabaseCommand> databaseCommand)
        {
            return ExecuteCommandAsync(databaseCommand.Data);
        }
    }

    public partial class DatabaseMessageExecutor
    {
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