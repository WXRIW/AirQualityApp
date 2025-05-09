using AirQualityApp.WinUI.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AirQualityApp.WinUI.Services
{
    public static class SettingsService
    {
        private static readonly string SettingsFilePath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AirQualityApp", "Settings.json");

        public static AppSettings Load()
        {
            if (!File.Exists(SettingsFilePath))
                return new AppSettings();

            var json = File.ReadAllText(SettingsFilePath);
            return JsonConvert.DeserializeObject<AppSettings>(json) ?? new AppSettings();
        }

        public static async Task<AppSettings> LoadAsync()
        {
            if (!File.Exists(SettingsFilePath))
                return new AppSettings();

            var json = await File.ReadAllTextAsync(SettingsFilePath);
            return JsonConvert.DeserializeObject<AppSettings>(json) ?? new AppSettings();
        }

        public static async Task SaveAsync(AppSettings settings)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SettingsFilePath)!);
            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            await File.WriteAllTextAsync(SettingsFilePath, json);
        }
    }
}
