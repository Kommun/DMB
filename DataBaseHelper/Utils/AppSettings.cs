using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.ComponentModel;
using Windows.ApplicationModel.Background;
using Windows.Storage.Pickers;

namespace Shared.Utils
{
    public class AppSettings : INotifyPropertyChanged
    {
        ApplicationDataContainer settings;
        public event PropertyChangedEventHandler PropertyChanged;

        public AppSettings()
        {
            settings = ApplicationData.Current.LocalSettings;
        }

        public void AddOrUpdateValue(string Key, Object value)
        {
            if (settings.Values[Key] != value)
                settings.Values[Key] = value;
            OnPropertyChanged("Key");
        }

        public T GetValueOrDefault<T>(string Key, T defaultValue)
        {
            T value;

            if (settings.Values.ContainsKey(Key))
                value = (T)settings.Values[Key];
            else
                value = defaultValue;

            return value;
        }

        private void OnPropertyChanged(string propertyName = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler == null)
                return;
            handler(null, new PropertyChangedEventArgs(propertyName));
        }

        #region Launch 

        public int Runs
        {
            get { return GetValueOrDefault<int>("Runs", 0); }
            set { AddOrUpdateValue("Runs", value); }
        }

        public bool NotRated
        {
            get { return GetValueOrDefault<bool>("NotRated", true); }
            set { AddOrUpdateValue("NotRated", value); }
        }

        public bool IsFirstLaunch
        {
            get { return GetValueOrDefault<bool>("IsFirstLaunch", true); }
            set { AddOrUpdateValue("IsFirstLaunch", value); }
        }

        #endregion

        #region BackgroundTask

        public string LastRemember
        {
            get { return GetValueOrDefault<string>("LastRemember", ""); }
            set { AddOrUpdateValue("LastRemember", value); }
        }

        public int notificationCountSetting
        {
            get { return GetValueOrDefault<int>("notificationCount", 0); }
            set { AddOrUpdateValue("notificationCount", value); }
        }

        #endregion

        #region SettingsView

        public bool IsRememberOn
        {
            get { return GetValueOrDefault<bool>("IsRememberOn", true); }
            set { AddOrUpdateValue("IsRememberOn", value); }
        }

        public int PercentDigitsNumber
        {
            get { return GetValueOrDefault<int>("PercentDigitsNumber", 0); }
            set { AddOrUpdateValue("PercentDigitsNumber", value); }
        }

        public int DisplayTimeFormat
        {
            get { return GetValueOrDefault<int>("DisplayTimeFormat", 0); }
            set { AddOrUpdateValue("DisplayTimeFormat", value); }
        }

        public int Theme
        {
            get { return GetValueOrDefault<int>("Theme", 0); }
            set
            {
                if (value == -1)
                    return;

                if (value == 6)
                    try
                    {
                        FileOpenPicker openPicker = new FileOpenPicker();
                        openPicker.ViewMode = PickerViewMode.Thumbnail;
                        openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                        openPicker.FileTypeFilter.Add(".jpg");
                        openPicker.FileTypeFilter.Add(".jpeg");
                        openPicker.FileTypeFilter.Add(".png");
                        openPicker.FileTypeFilter.Add(".bmp");

                        // Launch file open picker and caller app is suspended
                        // and may be terminated if required
                        openPicker.PickSingleFileAndContinue();
                    }
                    catch { }

                AddOrUpdateValue("Theme", value);
            }
        }

        #endregion

        #region Other

        public string dbVersionSetting
        {
            get { return GetValueOrDefault<string>("dbVersionSetting", "1"); }
            set { AddOrUpdateValue("dbVersionSetting", value); }
        }

        public int mainSoldierId
        {
            get { return GetValueOrDefault<int>("mainSoldierId", 0); }
            set { AddOrUpdateValue("mainSoldierId", value); }
        }

        public bool IsFullVersion
        {
            get { return GetValueOrDefault<bool>("IsFullVersion", false); }
            set { AddOrUpdateValue("IsFullVersion", value); }
        }

        public int AppVersion
        {
            get { return GetValueOrDefault<int>("AppVersion", 0); }
            set { AddOrUpdateValue("AppVersion", value); }
        }

        #endregion
    }
}