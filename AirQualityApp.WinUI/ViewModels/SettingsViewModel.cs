using AirQualityApp.WinUI.Models;
using AirQualityApp.WinUI.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace AirQualityApp.WinUI.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private AppSettings Settings => App.CurrentSettings;

        public event PropertyChangedEventHandler? PropertyChanged;

        #region App theme

        public int ThemeModeIndex
        {
            get => Settings.IsLightTheme switch
            {
                null => 2,
                true => 0,
                false => 1
            };
            set
            {
                Settings.IsLightTheme = value switch
                {
                    0 => true,
                    1 => false,
                    2 => null,
                    _ => null,
                };
                NotifyThemePropertiesChanged();
                _ = SaveAsync();
            }
        }

        public bool IsSystemDefaultTheme
        {
            get => Settings.IsLightTheme == null;
            set
            {
                if (value)
                {
                    Settings.IsLightTheme = null;
                    NotifyThemePropertiesChanged();
                    _ = SaveAsync();
                }
            }
        }

        public bool IsLightTheme
        {
            get => Settings.IsLightTheme == true;
            set
            {
                if (value)
                {
                    Settings.IsLightTheme = true;
                    NotifyThemePropertiesChanged();
                    _ = SaveAsync();
                }
            }
        }

        public bool IsDarkTheme
        {
            get => Settings.IsLightTheme == false;
            set
            {
                if (value)
                {
                    Settings.IsLightTheme = false;
                    NotifyThemePropertiesChanged();
                    _ = SaveAsync();
                }
            }
        }

        private void NotifyThemePropertiesChanged()
        {
            OnPropertyChanged(nameof(ThemeModeIndex));
            OnPropertyChanged(nameof(IsSystemDefaultTheme));
            OnPropertyChanged(nameof(IsLightTheme));
            OnPropertyChanged(nameof(IsDarkTheme));
            App.ApplyTheme();
        }

        #endregion

        #region Server

        public bool IsUseGlobalServer
        {
            get => Settings.IsUseMainlandServer == false;
            set
            {
                if (value)
                {
                    Settings.IsUseMainlandServer = false;
                    NotifyServerPropertiesChanged();
                    _ = SaveAsync();
                }
            }
        }

        public bool IsUseMainlandServer
        {
            get => Settings.IsUseMainlandServer == true;
            set
            {
                if (value)
                {
                    Settings.IsUseMainlandServer = true;
                    NotifyServerPropertiesChanged();
                    _ = SaveAsync();
                }
            }
        }

        public bool IsUseCustomServer
        {
            get => Settings.IsUseMainlandServer == null;
            set
            {
                if (value)
                {
                    Settings.IsUseMainlandServer = null;
                    NotifyServerPropertiesChanged();
                    _ = SaveAsync();
                }
            }
        }

        public string CustomServerUrl
        {
            get => Settings.CustomServerUrl ?? string.Empty;
            set
            {
                if (value != Settings.CustomServerUrl)
                {
                    Settings.CustomServerUrl = string.IsNullOrWhiteSpace(value) ? null : value;
                    NotifyServerPropertiesChanged();
                    _ = SaveAsync();
                }
            }
        }

        private void NotifyServerPropertiesChanged()
        {
            OnPropertyChanged(nameof(IsUseGlobalServer));
            OnPropertyChanged(nameof(IsUseMainlandServer));
            OnPropertyChanged(nameof(IsUseCustomServer));
            OnPropertyChanged(nameof(CustomServerUrl));
            App.ApplyServer();
        }

        #endregion

        public void Load()
        {
            NotifyThemePropertiesChanged();
            NotifyServerPropertiesChanged();
        }

        private async Task SaveAsync()
        {
            await SettingsService.SaveAsync(Settings);
        }

        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
