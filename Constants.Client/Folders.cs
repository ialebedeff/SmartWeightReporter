namespace Constants.Client
{
    public class Folders
    {
        public static string Builds => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "SmartWeight", "Builds");
        
    }
}