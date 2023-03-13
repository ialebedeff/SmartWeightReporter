namespace Entities
{
    public class RemoteBuilds
    { 
        public string Server { get; set; } = null!;
        public string BuildsPath { get; set; } = null!;
        public string BuildsFullPath => Path.Combine(Server, BuildsPath);
    }
}