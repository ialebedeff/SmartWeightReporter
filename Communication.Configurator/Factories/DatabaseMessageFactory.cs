using Entities;
using System.Text;

namespace Communication.Configurator
{
    public class DatabaseMessageFactory : IMessageFactory<DatabaseCommand, DatabaseCommand>
    {
        public Message<DatabaseCommand> CreateMessage(Factory to, DatabaseCommand param)
        {
            return new Message<DatabaseCommand>(to, param, string.Empty);
        }

        public Message<DatabaseCommand> CreateSelectWeighingsCommand(
            Factory to,
            DatabaseConnection databaseConnection, Filter filter)
        {
            var command = new DatabaseCommand() 
            { 
                Connection = databaseConnection
            };

            command
                .Select()
                .All()
                .From()
                .Database(databaseConnection.Database)
                .Table("weighings");

            if (!string.IsNullOrEmpty(filter.Contragent))
                command.InnerJoin("contragents AS c ON c.id = recipient_contragent_id");

            if (filter.From is null)
            {
                command.Where($"datetime_first > '{DateTime.MinValue.ToString("yyyy-MM-dd hh:mm:ss")}'");
            }
            else
            {
                command.Where($"datetime_first > '{filter.From.Value.ToString("yyyy-MM-dd hh:mm:ss")}'");
            }

            if (filter.To is null)
            {
                command.And($"end_datetime < '{DateTime.MaxValue.ToString("yyyy-MM-dd hh:mm:ss")}'");
            }
            else
            {
                command.And($"end_datetime < '{filter.To.Value.ToString("yyyy-MM-dd hh:mm:ss")}'");
            }


            if (!string.IsNullOrEmpty(filter.CarNumber))
                command.And($"transport_number LIKE '%{filter.CarNumber}%'");
            if (!string.IsNullOrEmpty(filter.Contragent))
                command.And($"c.name LIKE '%{filter.Contragent}%'");

            return CreateMessage(to, command);
        }
        public Message<DatabaseCommand> CreateSelectWorkCarsCommand(
            Factory to)
        {
            var command = new DatabaseCommand(to);
            var database = to.DatabaseConnection.Database;

            command.Command = $"SELECT * FROM {database}.work_cars as car " +
                               "LEFT JOIN drivers as driver " +
                               "ON driver.id = car.driver_id;";

            return CreateMessage(to, command);
        }

        public Message<DatabaseCommand> CreateSearchWorkCarsCommand(
            string query, Factory factory)
        {
            var command = new DatabaseCommand(factory);
            var database = factory.DatabaseConnection.Database;

            command.Command = $"SELECT * FROM {database}.work_cars as car " +
                               "LEFT JOIN drivers as driver " +
                               "ON driver.id = car.driver_id " +
                              $"WHERE car.transport_number LIKE \"%{query}%\";";

            return CreateMessage(factory, command);
        }
    }

    public static class DatabaseCommandsExtensions
    {
        public static DatabaseCommand Select(this DatabaseCommand databaseCommand) => Append(databaseCommand, "SELECT");
        public static DatabaseCommand Select(this DatabaseCommand databaseCommand, string condition) => Append(databaseCommand, $"SELECT {condition}");
        public static DatabaseCommand Database(this DatabaseCommand databaseCommand, string database) => Append(databaseCommand, database, ".");
        public static DatabaseCommand All(this DatabaseCommand databaseCommand) => Append(databaseCommand, "*");
        public static DatabaseCommand From(this DatabaseCommand databaseCommand) => Append(databaseCommand, "From");
        public static DatabaseCommand Table(this DatabaseCommand databaseCommand, string table) => Append(databaseCommand, table);
        public static DatabaseCommand Where(this DatabaseCommand databaseCommand, string condition) => Append(databaseCommand, $"WHERE {condition}");
        public static DatabaseCommand OrderBy(this DatabaseCommand databaseCommand, string condition) => Append(databaseCommand, $"ORDER BY {condition}");
        public static DatabaseCommand And(this DatabaseCommand databaseCommand, string condition) => Append(databaseCommand, $"AND {condition}");
        public static DatabaseCommand InnerJoin(this DatabaseCommand databaseCommand, string condition) => Append(databaseCommand, $"INNER JOIN {condition}");

        private static DatabaseCommand Append(DatabaseCommand databaseCommand, string value, string separator = " ")
        {
            if (databaseCommand is null)
            { 
                databaseCommand = new DatabaseCommand();
            }

            StringBuilder stringBuilder = new StringBuilder(databaseCommand.Command);
            stringBuilder.Append(value);
            stringBuilder.Append(separator);

            databaseCommand.Command = stringBuilder.ToString();

            return databaseCommand;
        }
    }
}