using AirQualityApp.Server.Helpers;
using AirQualityApp.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace AirQualityApp.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AirQualityController : ControllerBase
    {
        /// <summary>
        /// 测试 API 连通性
        /// </summary>
        [HttpGet("connection-test")]
        public IActionResult ConnectionTest()
        {
            return Ok("OK");
        }

        /// <summary>
        /// 获取指定城市的当前空气质量数据
        /// </summary>
        /// <param name="cityName">城市名，如 Shanghai</param>
        /// <returns>返回指定地区的空气质量数据，若无数据则返回 <see langword="null"</returns>
        [HttpGet("data/{cityName}/now")]
        [ProducesResponseType(typeof(AirQualityCityData), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public AirQualityCityData? GetCurrentAirQualityDataByCity(string cityName)
        {
            try
            {
                var latestFile = AirQualityControllerHelper.GetCityDataFiles(cityName).FirstOrDefault();
                if (latestFile == default || string.IsNullOrEmpty(latestFile.FilePath))
                {
                    return null;
                }
                var cityData = AirQualityControllerHelper.ReadCityData(latestFile.FilePath);
                return cityData;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取指定城市的空气质量数据
        /// </summary>
        /// <param name="cityName">城市名，如 Shanghai</param>
        /// <param name="limitDays">限制最大返回天数，若为 -1 则返回所有数据</param>
        /// <returns>返回指定城市的空气质量数据，若无数据则返回 <see langword="null"/></returns>
        [HttpGet("data/{cityName}")]
        [ProducesResponseType(typeof(List<AirQualityCityData?>), StatusCodes.Status200OK)]
        public List<AirQualityCityData?> GetAirQualityDataByCity(string cityName, [FromQuery] int limitDays = 3)
        {
            var results = new List<AirQualityCityData?>();
            try
            {
                var files = AirQualityControllerHelper.GetCityDataFiles(cityName, limitDays);

                foreach (var fileInfo in files)
                {
                    var cityData = AirQualityControllerHelper.ReadCityData(fileInfo.FilePath);
                    results.Add(cityData);
                }
                return results;
            }
            catch (NotSupportedException)
            {
                // 城市不支持，返回空列表
                return [];
            }
            catch (Exception ex) // 捕获文件访问等意外错误
            {
                Console.Error.WriteLine($"[控制器错误] 获取城市 '{cityName}' 历史数据时发生意外错误: {ex.Message}");
                return []; // 发生错误时返回空列表
            }
        }

        /// <summary>
        /// 获取指定城市的空气质量数据（基于指定日期往前数天）。
        /// </summary>
        /// <param name="cityName">城市名，如 Shanghai</param>
        /// <param name="startDate">作为范围结束日期的日期（包含）</param>
        /// <param name="limitDays">限制返回数据的天数（基于 <paramref name="startDate"/> 往前数），默认为 3。</param>
        /// <returns>返回 AirQualityCityData 对象的列表。</returns>
        [HttpGet("data/{cityName}/range")]
        [ProducesResponseType(typeof(List<AirQualityCityData?>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public List<AirQualityCityData?> GetAirQualityDataByCity(string cityName, [FromQuery] DateTime startDate, [FromQuery] int limitDays = 3)
        {
            if (limitDays <= 0 && limitDays != -1)
            {
                Console.WriteLine($"[控制器警告] 城市 '{cityName}' range 请求的 limitDays 参数无效: {limitDays}");
                return [];
            }

            var results = new List<AirQualityCityData?>();
            try
            {
                var files = AirQualityControllerHelper.GetCityDataFiles(cityName, limitDays, startDate);

                // 如果没有找到任何文件，直接返回空列表
                if (files.Count == 0)
                {
                    Console.WriteLine($"[控制器信息] 城市 '{cityName}' 日期范围数据请求未找到文件。范围: 结束日期 {startDate.Date:yyyy-MM-dd}, 天数 {limitDays}.");
                    return [];
                }

                foreach (var fileInfo in files)
                {
                    var cityData = AirQualityControllerHelper.ReadCityData(fileInfo.FilePath);
                    results.Add(cityData);
                }
                return results;
            }
            catch (NotSupportedException ex)
            {
                // 城市不支持，返回空列表或 NotFound
                Console.Error.WriteLine($"[控制器错误] 城市 '{cityName}' 日期范围数据请求失败，城市不受支持: {ex.Message}");
                return [];
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[控制器错误] 获取城市 '{cityName}' 日期范围数据时发生意外错误: {ex.Message}");
                return [];
            }
        }

        /// <summary>
        /// 获取指定城市指定地区编号的当前空气质量数据
        /// </summary>
        /// <param name="cityName">城市名，如 Shanghai</param>
        /// <param name="areaId">地区编号，如 201</param>
        /// <param name="limitDays">限制最大返回天数，若为 -1 则返回所有数据</param>
        /// <returns>返回指定地区的空气质量数据，若无数据则返回 <see langword="null"</returns>
        [HttpGet("data/{cityName}/{areaId}/now")]
        [ProducesResponseType(typeof(AirQualityAreaData), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public AirQualityAreaData? GetCurrentAirQualityAreaDataByCity(string cityName, int areaId)
        {
            try
            {
                var cityData = GetCurrentAirQualityDataByCity(cityName);

                if (cityData == null || cityData.Areas == null)
                {
                    return null;
                }

                // 在最新城市数据的 Areas 列表中查找指定 areaId 的地区数据
                var areaData = cityData.Areas.FirstOrDefault(a => a.Area?.Id == areaId);

                return areaData;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Unexpected error in GetCurrentAirQualityAreaDataByCity for {cityName}/{areaId}: {ex.Message}");
                return null; // 发生未知错误，返回 null
            }
        }

        /// <summary>
        /// 获取指定城市指定地区编号的空气质量数据
        /// </summary>
        /// <param name="cityName">城市名，如 Shanghai</param>
        /// <param name="areaId">地区编号，如 201</param>
        /// <param name="limitDays">限制最大返回天数，若为 -1 则返回所有数据</param>
        [HttpGet("data/{cityName}/{areaId}")]
        [ProducesResponseType(typeof(List<AirQualityAreaData>), StatusCodes.Status200OK)]
        public List<AirQualityAreaData> GetAirQualityAreaDataByCity(string cityName, int areaId, int limitDays = 3)
        {
            var results = new List<AirQualityAreaData>();
            try
            {
                var files = AirQualityControllerHelper.GetCityDataFiles(cityName, limitDays);

                foreach (var fileInfo in files)
                {
                    var cityData = AirQualityControllerHelper.ReadCityData(fileInfo.FilePath);

                    // 检查城市数据是否成功读取且包含 Areas 列表
                    if (cityData != null && cityData.Areas != null)
                    {
                        // 在当前文件的 Areas 列表中查找指定 areaId 的地区数据
                        var areaData = cityData.Areas.FirstOrDefault(a => a.Area?.Id == areaId);
                        if (areaData != null)
                        {
                            results.Add(areaData);
                        }
                    }

                }
            }
            catch (NotSupportedException ex)
            {
                Console.WriteLine(ex.Message);
                // 城市不支持，返回空列表
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Unexpected error in GetAirQualityAreaDataByCity for {cityName}/{areaId}: {ex.Message}");
                // 发生未知错误，返回当前已收集到的结果（可能为空列表）
            }

            return results;
        }

        /// <summary>
        /// 批量获取指定城市多个地区编号的历史空气质量数据
        /// </summary>
        /// <param name="cityName">城市名 (来自路径)</param>
        /// <param name="request">包含地区编号列表和限制天数的请求体 (来自 Body)</param>
        /// <returns>返回按时间戳分组的地区空气质量数据列表。每个元素是一个元组 (Timestamp, List of AreaData)。如果城市不支持、请求无效或无数据则返回空列表。</returns>
        [HttpPost("data/{cityName}/batch")]
        [ProducesResponseType(typeof(List<(DateTime, List<AirQualityAreaData>)>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public List<(DateTime Timestamp, List<AirQualityAreaData> Data)> GetAirQualityAreasDataByCity(string cityName, [FromBody] List<int> areaIds, int limitDays = 3)
        {
            if (areaIds == null || !areaIds.Any())
            {
                return [];
            }
            var results = new List<(DateTime Timestamp, List<AirQualityAreaData> Data)>();
            try
            {
                var files = AirQualityControllerHelper.GetCityDataFiles(cityName, limitDays);
                if (!files.Any())
                {
                    return [];
                }

                var requestedAreaIds = new HashSet<int>(areaIds);

                foreach (var fileInfo in files)
                {
                    var cityData = AirQualityControllerHelper.ReadCityData(fileInfo.FilePath);

                    if (cityData != null && cityData.Areas != null && cityData.Date != default)
                    {
                        // 过滤: 查找所有匹配请求ID的区域数据
                        var matchingAreas = cityData.Areas
                            .Where(a => a.Area != null && requestedAreaIds.Contains(a.Area.Id))
                            .ToList();

                        // 聚合: 如果找到了至少一个匹配项
                        if (matchingAreas.Any())
                        {
                            // 直接将元组添加到结果列表
                            results.Add((cityData.Date, matchingAreas));
                        }

                    }

                }
            }
            catch (NotSupportedException ex) // 处理城市不支持的异常 
            {
                return [];
            }
            catch (Exception ex) // 处理其他意外异常
            {
                Console.Error.WriteLine($"[控制器致命错误] 处理城市 '{cityName}' 的批量请求时发生意外错误: {ex.Message}"); // 保留基本错误输出
                // 返回空列表
                return [];
            }

            return results;
        }

        /// <summary>
        /// 获取所有城市的列表
        /// </summary>
        [HttpGet("areas")]
        [ProducesResponseType(typeof(List<CityInfo>), StatusCodes.Status200OK)]
        public List<CityInfo> GetCities()
        {
            return [new CityInfo("Shanghai", "上海市")];
        }

        /// <summary>
        /// 获取指定城市的地区列表
        /// </summary>
        /// <param name="cityName">城市名</param>
        /// <returns>指定城市的地区列表</returns>
        [HttpGet("areas/{cityName}")]
        [ProducesResponseType(typeof(List<AreaInfo>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public List<AreaInfo> GetAreaListByCity(string cityName)
        {
            try
            {
                var latestFile = AirQualityControllerHelper.GetCityDataFiles(cityName).FirstOrDefault();

                if (latestFile == default || string.IsNullOrEmpty(latestFile.FilePath))
                {
                    Console.WriteLine($"No data file found to retrieve area list for city: {cityName}");
                    // 返回空列表表示无法获取地区信息
                    return [];
                }

                var cityData = AirQualityControllerHelper.ReadCityData(latestFile.FilePath);

                // 检查数据是否成功读取以及是否包含 Areas 列表
                if (cityData == null || cityData.Areas == null)
                {
                    Console.WriteLine($"Failed to read or parse areas from latest data file for city: {cityName}");
                    // 返回空列表表示无法解析出地区信息
                    return [];
                }

                // 从 Areas 列表中提取 AreaInfo 对象
                var areaInfos = cityData.Areas
                    .Where(a => a.Area != null)
                    .Select(a => a.Area!)
                    .DistinctBy(ai => ai.Id)
                    .OrderBy(ai => ai.Id)
                    .ToList();

                return areaInfos;
            }
            catch (NotSupportedException ex)
            {
                Console.WriteLine(ex.Message);
                // 城市不支持，返回空列表
                return [];
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Unexpected error in GetAreaListByCity for {cityName}: {ex.Message}");
                // 发生未知错误，返回空列表
                return [];
            }
        }
    }
}
