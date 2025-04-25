using AirQualityApp.Shared.Models;
using Newtonsoft.Json;

namespace AirQualityApp.Api.Web
{
    public static class Areas
    {
        private static HttpClient Client => ServerHelper.Client;

        /// <summary>
        /// 获取支持的城市列表
        /// </summary>
        /// <returns>支持的城市列表</returns>
        public static async Task<List<CityInfo>> GetCities()
        {
            var responseString = await Client.GetStringAsync($"{ServerDefine.ServerUrl}/areas");
            var result = JsonConvert.DeserializeObject<List<CityInfo>>(responseString);
            return result!;
        }
    }
}
