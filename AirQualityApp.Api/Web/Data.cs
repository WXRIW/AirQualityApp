using AirQualityApp.Shared.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace AirQualityApp.Api.Web
{
    public static class Data
    {
        private static HttpClient Client => ServerHelper.Client;

        /// <summary>
        /// 获取指定城市的当前空气质量数据
        /// </summary>
        /// <param name="cityName">城市名，如 Shanghai</param>
        /// <returns>返回指定地区的空气质量数据，若无数据则返回 <see langword="null"</returns>
        public static async Task<AirQualityCityData> GetCurrentAirQualityDataByCity(string cityName)
        {
            var responseString = await Client.GetStringAsync($"{ServerDefine.ServerUrl}/data/{cityName}/now");
            var result = JsonConvert.DeserializeObject<AirQualityCityData>(responseString);
            return result!;
        }

        /// <summary>
        /// 获取指定城市的空气质量数据
        /// </summary>
        /// <param name="cityName">城市名，如 Shanghai</param>
        /// <param name="limitDays">限制最大返回天数，若为 -1 则返回所有数据</param>
        /// <returns>返回指定城市的空气质量数据，若无数据则返回 <see langword="null"/></returns>
        public static async Task<List<AirQualityCityData?>> GetAirQualityDataByCity(string cityName, int limitDays = 3)
        {
            var responseString = await Client.GetStringAsync($"{ServerDefine.ServerUrl}/data/{cityName}?limitDays={limitDays}");
            var result = JsonConvert.DeserializeObject<List<AirQualityCityData?>>(responseString);
            return result!;
        }

        /// <summary>
        /// 获取指定城市指定地区编号的当前空气质量数据
        /// </summary>
        /// <param name="cityName">城市名，如 Shanghai</param>
        /// <param name="areaId">地区编号，如 201</param>
        /// <param name="limitDays">限制最大返回天数，若为 -1 则返回所有数据</param>
        /// <returns>返回指定地区的空气质量数据，若无数据则返回 <see langword="null"</returns>
        public static async Task<AirQualityAreaData> GetCurrentAirQualityAreaDataByCity(string cityName, int areaId)
        {
            var responseString = await Client.GetStringAsync($"{ServerDefine.ServerUrl}/data/{cityName}/{areaId}/now");
            var result = JsonConvert.DeserializeObject<AirQualityAreaData?>(responseString);
            return result!;
        }

        /// <summary>
        /// 获取指定城市指定地区编号的空气质量数据
        /// </summary>
        /// <param name="cityName">城市名，如 Shanghai</param>
        /// <param name="areaId">地区编号，如 201</param>
        /// <param name="limitDays">限制最大返回天数，若为 -1 则返回所有数据</param>
        public static async Task<List<AirQualityAreaData?>> GetAirQualityAreaDataByCity(string cityName, int areaId, int limitDays = 3)
        {
            var responseString = await Client.GetStringAsync($"{ServerDefine.ServerUrl}/data/{cityName}/{areaId}?limitDays={limitDays}");
            var result = JsonConvert.DeserializeObject<List<AirQualityAreaData?>>(responseString);
            return result!;
        }

        /// <summary>
        /// 批量获取指定城市多个地区编号的历史空气质量数据
        /// </summary>
        /// <param name="cityName">城市名 (来自路径)</param>
        /// <param name="request">包含地区编号列表和限制天数的请求体 (来自 Body)</param>
        /// <returns>返回按时间戳分组的地区空气质量数据列表。每个元素是一个元组 (Timestamp, List of AreaData)。如果城市不支持、请求无效或无数据则返回空列表。</returns>
        public static async Task<List<(DateTime, List<AirQualityAreaData>)>> PostAirQualityAreasDataByCity(string cityName, int areaId, int limitDays)
        {
            var requestBody = new
            {
                AreaIds = new List<int> { areaId },
                LimitDays = limitDays
            };

            var jsonContent = new StringContent(
                JsonConvert.SerializeObject(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            try
            {
                var response = await Client.PostAsync($"{ServerDefine.ServerUrl}/data/{cityName}/batch", jsonContent);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[客户端错误] 请求失败，状态码: {response.StatusCode}");
                    return [];
                }

                var responseString = await response.Content.ReadAsStringAsync();

                // 服务端返回的是对象列表，每个对象形如：
                // { "timestamp": "2024-05-05T12:00:00", "data": [ ... ] }
                var jArray = JArray.Parse(responseString);
                var result = new List<(DateTime, List<AirQualityAreaData>)>();

                foreach (var item in jArray)
                {
                    var timestamp = item["timestamp"]!.ToObject<DateTime>();
                    var dataList = item["data"]!.ToObject<List<AirQualityAreaData>>();
                    result.Add((timestamp, dataList!));
                }

                return result;
            }
            catch
            {
                return [];
            }
        }

        /// <summary>
        /// 获取所有城市的列表
        /// </summary>
        public static async Task<List<CityInfo?>> GetCities()
        {
            var responseString = await Client.GetStringAsync($"{ServerDefine.ServerUrl}/areas");
            var result = JsonConvert.DeserializeObject<List<CityInfo?>>(responseString);
            return result!;
        }

        /// <summary>
        /// 获取指定城市的地区列表
        /// </summary>
        /// <param name="cityName">城市名</param>
        /// <returns>指定城市的地区列表</returns>
        public static async Task<List<AreaInfo?>> GetAreaListByCity(string cityName)
        {
            var responseString = await Client.GetStringAsync($"{ServerDefine.ServerUrl}/areas/{cityName}");
            var result = JsonConvert.DeserializeObject<List<AreaInfo?>>(responseString);
            return result!;
        }
    }
}

