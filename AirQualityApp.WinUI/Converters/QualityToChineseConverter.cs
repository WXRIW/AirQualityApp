using AirQualityApp.Shared.Models;
using Microsoft.UI.Xaml.Data;
using System;

namespace AirQualityApp.WinUI.Converters
{
    public class QualityToChineseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value switch
            {
                AirQualityLevel.Excellent => "优",
                AirQualityLevel.Good => "良",
                AirQualityLevel.LightlyPolluted => "轻度污染",
                AirQualityLevel.ModeratelyPolluted => "中度污染",
                AirQualityLevel.HeavilyPolluted => "重度污染",
                AirQualityLevel.SeverelyPolluted => "严重污染",
                _ => "未知"
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
    }

}
