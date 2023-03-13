using System.Text.Json.Serialization;

namespace Entities
{
    public class BinaryFileInformation : EntityBase<int>
    {
        public string RelativePath { get; set; } = null!;
        public string AbsolutePath { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public string? Version { get; set; }
        public string Hash { get; set; } = null!;

        [JsonIgnore]
        public Build? Build { get; set; }
    }
}