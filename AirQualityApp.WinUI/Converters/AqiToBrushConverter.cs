using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;

namespace AirQualityApp.WinUI.Converters
{
    public class AqiToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int aqi)
            {
                bool isDark;
                if (App.CurrentSettings.IsLightTheme != null)
                {
                    isDark = App.CurrentSettings.IsLightTheme == false;
                }
                else
                {
                    isDark = App.Current.RequestedTheme == Microsoft.UI.Xaml.ApplicationTheme.Dark;
                }

                if (aqi <= 50)
                    return new SolidColorBrush(isDark ? Windows.UI.Color.FromArgb(255, 144, 238, 144) : Windows.UI.Color.FromArgb(255, 0, 128, 0)); // 优
                else if (aqi <= 100)
                    return new SolidColorBrush(isDark ? Windows.UI.Color.FromArgb(255, 255, 255, 102) : Colors.Orange); // 良
                else if (aqi <= 150)
                    return new SolidColorBrush(isDark ? Windows.UI.Color.FromArgb(255, 255, 153, 102) : Windows.UI.Color.FromArgb(255, 255, 165, 0)); // 轻度污染
                else if (aqi <= 200)
                    return new SolidColorBrush(isDark ? Windows.UI.Color.FromArgb(255, 255, 102, 102) : Windows.UI.Color.FromArgb(255, 255, 0, 0)); // 中度污染
                else if (aqi <= 300)
                    return new SolidColorBrush(isDark ? Windows.UI.Color.FromArgb(255, 204, 102, 255) : Windows.UI.Color.FromArgb(255, 153, 50, 204)); // 重度污染
                else
                    return new SolidColorBrush(isDark ? Windows.UI.Color.FromArgb(255, 255, 102, 204) : Windows.UI.Color.FromArgb(255, 139, 0, 0)); // 严重污染
            }

            return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
    }

}
