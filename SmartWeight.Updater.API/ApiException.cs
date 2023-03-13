namespace SmartWeight.Updater.API
{
    public class ApiException : Exception
    {
        public ApiException(string message) : base(message) { }
        public static void ThrowIfFalse(bool value, string message)
        {
            if (value is false) throw new ApiException(message);
        }
    }
}