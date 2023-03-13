namespace Entities
{
    public class Build : EntityBase<int>
    {
        public List<BinaryFileInformation> Binaries { get; set; } = new();
    }
}