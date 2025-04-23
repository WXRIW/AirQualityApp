namespace AirQualityApp.Server.Helpers
{
    /// <summary>
    /// 自动下载帮助类
    /// </summary>
    public class AutoDownloadHelper
    {
        private readonly System.Timers.Timer _timer;

        public AutoDownloadHelper()
        {
            _timer = new(1000 * 10 * 60); // 每 10 分钟执行一次
            _timer.Elapsed += Timer_Elapsed;
        }

        public void Start()
        {
            _timer.Start();
            Timer_Elapsed(null, null!);
        }

        public void Stop()
        {
            _timer.Stop();
        }

        private async void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            // 调用 DataDownloadHelper 中的下载方法
            await DataDownloadHelper.DownloadLatest();
        }
    }
}
