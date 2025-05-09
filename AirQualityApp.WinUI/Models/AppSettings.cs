using Newtonsoft.Json;

namespace AirQualityApp.WinUI.Models
{
    public class AppSettings
    {
        [JsonProperty("isLightTheme")]
        public bool? IsLightTheme { get; set; }

        /// <summary>
        /// 是否使用大陆服务器<br/>
        /// <see langword="null"/> 指使用自定义服务器
        /// </summary>
        [JsonProperty("isUseMainlandServer")]
        public bool? IsUseMainlandServer { get; set; } = false;

        [JsonProperty("customServerUrl")]
        public string? CustomServerUrl { get; set; }
    }
}
