using AirQualityApp.WinUI.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System.Collections.Generic;

namespace AirQualityApp.WinUI
{
    public sealed partial class MainWindow : Window
    {
        private readonly Dictionary<string, Page> _pageCache = new();

        public MainWindow()
        {
            InitializeComponent();
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);

            NavView.SelectedItem = NavView.MenuItems[0];

            AppWindow.SetIcon("Assets/Icon.ico");
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is NavigationViewItem item && item.Tag is string tag)
            {
                NavigateToPage(tag);
            }
        }

        private void NavigateToPage(string tag)
        {
            if (!_pageCache.TryGetValue(tag, out var page))
            {
                page = tag switch
                {
                    "Home" => new HomePage(),
                    "Settings" => new SettingsPage(),
                    _ => null
                };

                if (page is null)
                    return;

                _pageCache[tag] = page;
            }

            if (ContentFrame.Content as Page != page)
            {
                AddEntranceAnimation(page);
                ContentFrame.Content = page;
            }
        }

        private void AddEntranceAnimation(Page page)
        {
            if (page.Content is FrameworkElement root)
            {
                root.Transitions =
                [
                    new EntranceThemeTransition()
                ];
            }
        }
    }
}