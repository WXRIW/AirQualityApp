namespace AirQualityApp.Api.Web
{
    public static class Connectivity
    {
        private static HttpClient Client => ServerHelper.Client;

        /// <summary>
        /// 检查服务器连接
        /// </summary>
        /// <returns>与服务器是否连通，联通则返回 <see langword="true"/>，否则返回 <see langword="false"/></returns>
        public static async Task<bool> IsConnected()
        {
            try
            {
                var responseString = await Client.GetStringAsync($"{ServerDefine.ServerUrl}/connection-test");
                return responseString == "OK";
            }
            catch
            {
                return false;
            }
        }
    }
}
