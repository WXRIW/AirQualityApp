namespace AirQualityApp.Api
{
    internal class ServerHelper
    {
        public static HttpClient Client { get; set; } = new();
    }
}
