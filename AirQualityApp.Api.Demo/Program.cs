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

            //Console.WriteLine("\nGet available cities:");
            //var cities = Web.Areas.GetCities().Result;
            //Console.WriteLine(JsonConvert.SerializeObject(cities, Formatting.Indented));

            //Console.WriteLine("\nGet Shanghai AQ now:");
            //var aqNow = Web.Data.GetCurrentAirQualityDataByCity("Shanghai").Result;
            //Console.WriteLine(JsonConvert.SerializeObject(aqNow, Formatting.Indented));

            Console.WriteLine("\nGet Shanghai AQ in 2 days:");
            var aq2Days = Web.Data.GetAirQualityDataByCity("Shanghai", 2).Result;
            Console.WriteLine(JsonConvert.SerializeObject(aq2Days, Formatting.Indented));

            //Console.WriteLine("\nGet Shanghai AQ in AreaId");
            //var aqAreaId = Web.Data.GetCurrentAirQualityAreaDataByCity("Shanghai", 201).Result;
            //Console.WriteLine(JsonConvert.SerializeObject(aqAreaId, Formatting.Indented));

            //Console.WriteLine("\nGet Shanghai AQ in AreaId in 2 days");
            //var aqAreaId2days = Web.Data.GetAirQualityAreaDataByCity("Shanghai", 201, 2).Result;
            //Console.WriteLine(JsonConvert.SerializeObject(aqAreaId2days, Formatting.Indented));

            //Console.WriteLine("\nGet Shanghai AQ in AreaId in 2 days (POST)");
            //var aqAreaId2Dayspost = Web.Data.PostAirQualityAreasDataByCity("Shanghai", 203, 2).Result;
            //Console.WriteLine(JsonConvert.SerializeObject(aqAreaId2Dayspost, Formatting.Indented));

            //Console.WriteLine("\nGet All Citys list");
            //var cityList = Web.Areas.GetCities().Result;
            //Console.WriteLine(JsonConvert.SerializeObject(cityList, Formatting.Indented));

            //Console.WriteLine("\nGet City Area list");
            //var areaList = Web.Areas.GetAreaListByCity("Shanghai").Result;
            //Console.WriteLine(JsonConvert.SerializeObject(areaList, Formatting.Indented));
        }
    }
}
