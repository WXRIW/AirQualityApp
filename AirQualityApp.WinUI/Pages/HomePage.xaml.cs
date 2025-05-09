using AirQualityApp.Shared.Models;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.UI.Xaml.Controls;
using SkiaSharp;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace AirQualityApp.WinUI.Pages
{
    public sealed partial class HomePage : Page
    {
        public static ObservableCollection<CityInfo> StaticCities { get; set; } = new();
        public static ObservableCollection<AreaInfo> StaticAreas { get; set; } = new();
        public static ObservableCollection<AirQualityAreaData> StaticCurrentAreaData { get; set; } = new();
        public static ObservableCollection<AirQualityCityData> StaticHistoricalCityData { get; set; } = new();


        public ObservableCollection<CityInfo> Cities
        {
            get => StaticCities;
            set => StaticCities = value;
        }
        public ObservableCollection<AreaInfo> Areas
        {
            get => StaticAreas;
            set => StaticAreas = value;
        }
        public ObservableCollection<AirQualityAreaData> CurrentAreaData
        {
            get => StaticCurrentAreaData;
            set => StaticCurrentAreaData = value;
        }
        public ObservableCollection<AirQualityCityData> HistoricalCityData
        {
            get => StaticHistoricalCityData;
            set => StaticHistoricalCityData = value;
        }

        public ISeries[] AQISeries { get; set; } = Array.Empty<ISeries>();
        public Axis[] AQIXAxes { get; set; } = Array.Empty<Axis>();
        public Axis[] AQIYAxes { get; set; } = new[] { new Axis { Name = "AQI", MinLimit = 0, MaxLimit = 500 } };

        public HomePage()
        {
            this.InitializeComponent();
            LoadInitialData();
        }

        private async void LoadInitialData(bool isForceRefresh = false)
        {
            try
            {
                //if (!await Connectivity.IsConnected())
                //{
                //    ShowError("网络未连接，请检查您的网络。");
                //    return;
                //}

                var cities = await Api.Web.Areas.GetCities();
                Cities.Clear();
                foreach (var city in cities)
                    Cities.Add(city);

                CityComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ShowError("加载城市数据失败: " + ex.Message);
            }
        }

        private async void CityComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CityComboBox.SelectedItem is CityInfo city)
            {
                try
                {
                    var areas = await Api.Web.Areas.GetAreaListByCity(city.Name);
                    Areas.Clear();
                    foreach (var area in areas)
                        Areas.Add(area);

                    AreaComboBox.SelectedIndex = 0;
                    await LoadCityData(city.Name);
                }
                catch (Exception ex)
                {
                    ShowError("加载地区数据失败: " + ex.Message);
                }
            }
        }

        private async void AreaComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CityComboBox.SelectedItem is CityInfo city &&
                AreaComboBox.SelectedItem is AreaInfo area)
            {
                try
                {
                    var data = await Api.Web.Data.GetCurrentAirQualityAreaDataByCity(city.Name, area.Id);
                    CurrentAreaData.Clear();
                    CurrentAreaData.Add(data);
                }
                catch (Exception ex)
                {
                    ShowError("加载空气质量数据失败: " + ex.Message);
                }
            }
        }

        private async Task LoadCityData(string cityName)
        {
            try
            {
                var history = await Api.Web.Data.GetAirQualityDataByCity(cityName, 7);
                HistoricalCityData.Clear();
                foreach (var item in history)
                {
                    if (item != null)
                        HistoricalCityData.Add(item);
                }

                var points = HistoricalCityData
                    .Where(d => d.Areas?.FirstOrDefault()?.Nodes?.FirstOrDefault()?.AirQuality?.AQI is int)
                    .Select(d => new ObservableValue(d.Areas.First().Nodes.First().AirQuality.AQI.Value))
                    .ToArray();

                AQISeries = new ISeries[]
                {
                    new LineSeries<ObservableValue>
                    {
                        Values = points,
                        Fill = new SolidColorPaint
                        {
                            Color = new SKColor(30, 144, 255, 80) // 半透明天蓝色
                        },
                        GeometrySize = 10
                    }
                };

                AQIXAxes = new[] {
                    new Axis { Labels = HistoricalCityData.Select(d => d.Date.ToString("MM-dd")).ToArray() }
                };
            }
            catch (Exception ex)
            {
                ShowError("加载历史数据失败: " + ex.Message);
            }
        }

        private void ShowError(string message)
        {
            ContentDialog dialog = new()
            {
                Title = "错误",
                Content = message,
                CloseButtonText = "确定",
                XamlRoot = this.Content.XamlRoot
            };
            _ = dialog.ShowAsync();
        }
    }
}
