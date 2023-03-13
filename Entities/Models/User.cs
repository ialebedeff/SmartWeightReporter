using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace Entities
{
    public class User : IdentityUser<int>
    {
        [JsonIgnore]
        public List<Factory> Factories { get; set; } = new();
        [JsonIgnore]
        public List<Note> Notes { get; set; } = new();
    }
}