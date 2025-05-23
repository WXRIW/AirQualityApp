using AirQualityApp.Shared.Models;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace AirQualityApp.WinUI.Pages
{
    public sealed partial class HomePage : Page
    {
        public ObservableCollection<CityInfo> Cities { get; set; } = [];
        public ObservableCollection<AreaInfo> Areas { get; set; } = [];
        public ObservableCollection<AirQualityAreaData> CurrentAreaData { get; set; } = [];
        public List<AirQualityCityData> HistoricalCityData { get; set; } = [];
        public ObservableCollection<AirQualityAreaData> CurrentHistoryAreaData { get; set; } = [];

        private DateTimeOffset _selectedDate = DateTimeOffset.Now;
        public DateTimeOffset SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (_selectedDate != value)
                {
                    _selectedDate = value;
                    LoadHistoricalData();
                }
            }
        }

        private TimeSpan _selectedTime = DateTimeOffset.Now.TimeOfDay
            .Add(TimeSpan.FromMinutes(-DateTimeOffset.Now.TimeOfDay.Minutes)
            .Add(TimeSpan.FromHours(-1)));
        public TimeSpan SelectedTime
        {
            get => _selectedTime;
            set
            {
                if (_selectedTime != value)
                {
                    _selectedTime = value;
                    LoadHistoricalData();
                }
            }
        }

        public HomePage()
        {
            this.InitializeComponent();
            LoadInitialData();
        }

        private async void LoadInitialData(bool isForceRefresh = false)
        {
            if (isForceRefresh)
            {
                Cities.Clear();
                Areas.Clear();
                CurrentAreaData.Clear();
                HistoricalCityData.Clear();
                CurrentHistoryAreaData.Clear();
            }

            try
            {
                //if (!await Connectivity.IsConnected())
                //{
                //    ShowError("网络未连接，请检查您的网络。");
                //    return;
                //}

                var cities = await Api.Web.Areas.GetCities();
                foreach (var city in cities)
                    Cities.Add(city);

                CityComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ShowError("加载城市数据失败: " + ex.Message);
            }
        }

        private void RefreshButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            LoadInitialData(true);
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
                        if (area != null)
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
                    LoadHistoricalData(false);
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
                // 先获取最近 2 天的数据
                var history = await Api.Web.Data.GetAirQualityDataByCity(cityName, 2);
                HistoricalCityData.Clear();
                CurrentHistoryAreaData.Clear();
                foreach (var item in history)
                {
                    if (item != null)
                        HistoricalCityData.Add(item);
                }
                LoadHistoricalData(false);
            }
            catch (Exception ex)
            {
                ShowError("加载历史数据失败: " + ex.Message);
            }
        }

        private void HistoryDatePicker_DateChanged(object sender, DatePickerValueChangedEventArgs args)
        {
            SelectedDate = args.NewDate;
        }

        private void HistoryTimePicker_TimeChanged(object sender, TimePickerValueChangedEventArgs args)
        {
            SelectedTime = args.NewTime;
        }

        private async void LoadHistoricalData(bool isShowFailedMessage = true)
        {
            if (CityComboBox.SelectedItem is not CityInfo city)
                return;

            try
            {
                CurrentHistoryAreaData.Clear();

                DateTime selectedDateTime = SelectedDate.Date + SelectedTime;
                // 在 HistoricalCityData 中找到与 selectedDateTime 匹配的项，并放到 CurrentHistoryAreaData 中
                var historyData = HistoricalCityData.FirstOrDefault(data => data.Date.Date == selectedDateTime.Date
                    && data.Date.Hour == selectedDateTime.Hour);
                if (historyData == null)
                {
                    // 可能是当前日期的数据没有被预获取，再往前获取 2 天
                    var history = await Api.Web.Data.GetAirQualityDataByCity(city.Name, selectedDateTime.Date, 2);
                    if (history != null)
                    {
                        foreach (var item in history)
                        {
                            if (item != null && !HistoricalCityData.Any(t => item.Date.Equals(t.Date)))
                                HistoricalCityData.Add(item);
                        }
                        historyData = HistoricalCityData.FirstOrDefault(data => data.Date.Date == selectedDateTime.Date
                            && data.Date.Hour == selectedDateTime.Hour);
                    }
                }
                if (historyData != null)
                {
                    var areaData = historyData.Areas.FirstOrDefault(area => area.Area.Id == (AreaComboBox.SelectedItem as AreaInfo)?.Id);
                    if (areaData != null)
                    {
                        CurrentHistoryAreaData.Clear();
                        CurrentHistoryAreaData.Add(areaData);
                    }
                }
                else if (isShowFailedMessage)
                {
                    ShowError("没有找到历史数据，请选择其他日期或时间。");
                }
                else
                {
                    CurrentHistoryAreaData.Clear();
                }
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
