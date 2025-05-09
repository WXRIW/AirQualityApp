namespace AirQualityApp.WinUI.Services
{
    public static class ServersService
    {
        public static void SetGlobalServer()
        {
            Api.ServerDefine.ServerUrlOverride = "https://api.weather.wxriw.cn/AirQuality";
        }

        public static void SetMainlandServer()
        {
            Api.ServerDefine.ServerUrlOverride = "https://cn.api.weather.wxriw.cn/AirQuality";
        }

        public static void SetCustomServer(string? server)
        {
            Api.ServerDefine.ServerUrlOverride = server;
        }

        public static void RemoveServerOverride()
        {
            Api.ServerDefine.ServerUrlOverride = null;
        }
    }
}
