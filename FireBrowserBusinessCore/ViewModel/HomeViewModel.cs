using CommunityToolkit.Mvvm.ComponentModel;
using FireBrowserBusinessCore.Models;
using Microsoft.UI.Xaml;
using System;
using System.Threading.Tasks;

namespace FireBrowserCore.ViewModel
{
    public partial class HomeViewModel : ObservableObject
    {

        private Settings.NewTabBackground _backgroundType;
        private string _imageTitle;
        private string _imageCopyright;
        private string _imageCopyrightLink;
        [ObservableProperty]
        private Visibility _ntpCoreVisibility;
        [ObservableProperty]
        private bool _isNtpTimeVisible;
        [ObservableProperty]
        private string _ntpTimeText;
        [ObservableProperty]
        private string _ntpDateText;
        [ObservableProperty]
        private bool _ntpTimeEnabled;
        private DispatcherTimer timer { get; set; }
        FireBrowserMultiCore.Settings userSettings { get; } = FireBrowserMultiCore.UserFolderManager.LoadUserSettings(FireBrowserMultiCore.AuthService.CurrentUser);

        private void UpdateUIControls()
        {

            NtpTimeEnabled = userSettings.NtpDateTime == "1";
            IsNtpTimeVisible = NtpTimeEnabled;
            NtpCoreVisibility = IsNtpTimeVisible ? Visibility.Visible : Visibility.Collapsed;
            OnPropertyChanged(nameof(NtpCoreVisibility));
            OnPropertyChanged(nameof(IsNtpTimeVisible));
            OnPropertyChanged(nameof(NtpTimeEnabled));
            userSettings.NtpDateTime = NtpTimeEnabled ? "1" : "0";
            FireBrowserMultiCore.UserFolderManager.SaveUserSettings(FireBrowserMultiCore.AuthService.CurrentUser, userSettings);


        }
        public HomeViewModel()
        {
            UpdateUIControls();
        }
        public Task Intialize()
        {

            NtpCoreVisibility = IsNtpTimeVisible ? Visibility.Visible : Visibility.Collapsed;
            OnPropertyChanged(nameof(NtpCoreVisibility));
            if (IsNtpTimeVisible)
            {
                InitClock();
            }
            return Task.CompletedTask;

        }
        private void UpdateClock()
        {

            (NtpTimeText, NtpDateText) = (DateTime.Now.ToString("H:mm"), $"{DateTime.Today.DayOfWeek}, {DateTime.Today.ToString("MMMM d")}");
            OnPropertyChanged(nameof(NtpTimeText));
            OnPropertyChanged(nameof(NtpDateText));

        }
        private void InitClock()
        {
            // intial time => use timer to update then after that.
            UpdateClock();
            timer = new DispatcherTimer();
            timer.Interval = new System.TimeSpan(0, 1, 0);
            timer.Tick += (_, _) =>
            {
                UpdateClock();
            };
            timer.Start();

        }

        public Settings.NewTabBackground BackgroundType
        {
            get => _backgroundType;
            set => SetProperty(ref _backgroundType, value);
        }

        public string ImageTitle
        {
            get => _imageTitle;
            set => SetProperty(ref _imageTitle, value);
        }

        public string ImageCopyright
        {
            get => _imageCopyright;
            set => SetProperty(ref _imageCopyright, value);
        }

        public string ImageCopyrightLink
        {
            get => _imageCopyrightLink;
            set => SetProperty(ref _imageCopyrightLink, value);
        }
    }
}