using Newtonsoft.Json;

namespace AirQualityApp.Api.Demo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Testing connectivity:");
            var connectionOk = Web.Connectivity.IsConnected().Result;
            Console.WriteLine(connectionOk ? "Connected" : "Not connected");

            Console.WriteLine("\nGet available cities:");
            var cities = Web.Areas.GetCities().Result;
            Console.WriteLine(JsonConvert.SerializeObject(cities, Formatting.Indented));
        }
    }
}
