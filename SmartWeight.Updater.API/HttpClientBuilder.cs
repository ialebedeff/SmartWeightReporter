using System.Net;

namespace SmartWeight.Updater.API
{
    public class HttpClientBuilder
    {
        public CookieContainer? CookieContainer { get; set; }
        public HttpClient HttpClient { get; set; } = null!;

        public HttpClient Build(
            string url,
            CookieContainer? cookieContainer = null!)
        {
            CookieContainer = cookieContainer;
            if (cookieContainer is not null)
            {
                HttpClient = new HttpClient(
                    new HttpClientHandler()
                    {
                        CookieContainer = cookieContainer
                    })
                {
                    BaseAddress = new Uri(url)
                };
            }
            else 
            {
                HttpClient = new HttpClient()
                {
                    BaseAddress = new Uri(url)
                };
            }

            return HttpClient;
        }
    }
}