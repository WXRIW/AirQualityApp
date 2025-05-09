namespace AirQualityApp.Api
{
    public static class ServerDefine
    {
        /// <summary>
        /// 服务器地址
        /// </summary>
        public static string ServerUrl => ServerUrlOverride ?? "http://127.0.0.1:5000/AirQuality";

        /// <summary>
        /// 服务器地址覆写
        /// </summary>
        public static string? ServerUrlOverride { get; set; } = null;
    }
}
