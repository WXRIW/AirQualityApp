using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;

namespace AirQualityApp.WinUI.Pages
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
            Loaded += (_, _) => ViewModel.Load();
        }

        public string Version
        {
            get
            {
                var version = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version!;
                return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
            }
        }

        private void ToCloneRepoCard_Click(object sender, RoutedEventArgs e)
        {
            DataPackage package = new DataPackage();
            package.SetText(gitCloneTextBlock.Text);
            Clipboard.SetContent(package);
        }

        private async void GitRepoLinkCard_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/WXRIW/AirQualityApp"));
        }

        private async void BugRequestCard_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/WXRIW/AirQualityApp/issues"));
        }
    }
}
