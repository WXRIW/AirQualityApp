using AirQualityApp.WinUI.Helpers;
using AirQualityApp.WinUI.Models;
using AirQualityApp.WinUI.Services;
using Microsoft.UI.Xaml;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AirQualityApp.WinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            CurrentSettings = await SettingsService.LoadAsync();
            LoadSettings(false);

            m_window = new MainWindow();
            LoadSettings(true);
            m_window.SetWindowSize(width: 900, height: 600);
            m_window.CenterOnScreen();
            m_window.Activate();
        }

        private Window? m_window;

        public static AppSettings CurrentSettings { get; private set; } = new();

        /// <summary>
        /// 加载设置
        /// </summary>
        /// <param name="isAfterLoad">是否是在创建窗口后需要调用的</param>
        public static void LoadSettings(bool isAfterLoad)
        {
            if (isAfterLoad)
            {
                ApplyTheme();
            }
            else
            {
                ApplyServer();
            }
        }

        public static void ApplyTheme()
        {
            var rootWindow = (Application.Current as App)?.m_window;

            if (rootWindow?.Content is FrameworkElement rootElement)
            {
                rootElement.RequestedTheme = CurrentSettings.IsLightTheme switch
                {
                    true => ElementTheme.Light,
                    false => ElementTheme.Dark,
                    null => ElementTheme.Default,
                };
                TitleBarHelper.ApplySystemThemeToCaptionButtons(rootWindow);
            }
        }

        public static void ApplyServer()
        {
            if (CurrentSettings.IsUseMainlandServer == true)
            {
                ServersService.SetMainlandServer();
            }
            else if (CurrentSettings.IsUseMainlandServer == null)
            {
                ServersService.SetCustomServer(CurrentSettings.CustomServerUrl);
            }
            else
            {
                ServersService.SetGlobalServer();
            }
        }
    }
}