using System.Text.Json.Serialization;

namespace Entities
{
    public class DatabaseConnection : EntityBase<int>
    {
        [JsonIgnore]
        public Factory Factory { get; set; } = null!;
        public int FactoryId { get; set; }
        public string Server { get; set; } = null!;
        public string Database { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string User { get; set; } = null!;
    }
}