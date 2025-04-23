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
        /// <returns>正常连通则显示 OK</returns>
        [HttpGet("connection-test")]
        public IActionResult ConnectionTest()
        {
            return Ok("OK");
        }

        /// <summary>
        /// 获取指定城市的空气质量数据
        /// </summary>
        /// <param name="cityName">城市名，如 Shanghai</param>
        /// <param name="limitDays">限制最大返回天数，若为 -1 则返回所有数据</param>
        /// <returns>返回指定城市的空气质量数据，若无数据则返回 <see langword="null"/></returns>
        [HttpGet("data/{cityName}")]
        public AirQualityCityData? GetAirQualityDataByCity(string cityName, int limitDays = 3)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取指定城市的当前空气质量数据
        /// </summary>
        /// <param name="cityName">城市名，如 Shanghai</param>
        /// <returns>返回指定地区的空气质量数据，若无数据则返回 <see langword="null"</returns>
        [HttpGet("data/{cityName}/now")]
        public AirQualityAreaData? GetCurrentAirQualityAreaDataByCity(string cityName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取指定城市指定地区编号的当前空气质量数据
        /// </summary>
        /// <param name="cityName">城市名，如 Shanghai</param>
        /// <param name="areaId">地区编号，如 201</param>
        /// <param name="limitDays">限制最大返回天数，若为 -1 则返回所有数据</param>
        /// <returns>返回指定地区的空气质量数据，若无数据则返回 <see langword="null"</returns>
        [HttpGet("data/{cityName}/{areaId}/now")]
        public AirQualityAreaData? GetCurrentAirQualityAreaDataByCity(string cityName, int areaId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取指定城市指定地区编号的空气质量数据
        /// </summary>
        /// <param name="cityName">城市名，如 Shanghai</param>
        /// <param name="areaId">地区编号，如 201</param>
        /// <param name="limitDays">限制最大返回天数，若为 -1 则返回所有数据</param>
        /// <returns>返回指定地区的空气质量数据，若无数据则返回 <see langword="null"</returns>
        [HttpGet("data/{cityName}/{areaId}")]
        public AirQualityAreaData? GetAirQualityAreaDataByCity(string cityName, int areaId, int limitDays = 3)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 批量获取指定城市指定地区编号的空气质量数据
        /// </summary>
        /// <param name="cityName">城市名</param>
        /// <param name="areaIds">地区编号列表</param>
        /// <param name="limitDays">限制最大返回天数</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpPost("data/{cityName}/batch")]
        public List<AirQualityAreaData> GetAirQualityAreasDataByCity(string cityName, List<int> areaIds, int limitDays = 3)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取所有城市的列表
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpGet("areas")]
        public List<CityInfo> GetCities()
        {
            return [new("Shanghai")];
        }

        /// <summary>
        /// 获取指定城市的地区列表
        /// </summary>
        /// <param name="cityName">城市名</param>
        /// <returns>指定城市的地区列表</returns>
        [HttpGet("areas/{cityName}")]
        public List<(int, string)> GetAreaListByCity(string cityName)
        {
            throw new NotImplementedException();
        }
    }
}
