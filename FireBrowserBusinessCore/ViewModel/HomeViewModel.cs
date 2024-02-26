using CommunityToolkit.Mvvm.ComponentModel;
using FireBrowserBusinessCore.Helpers;
using FireBrowserBusinessCore.Models;
using FireBrowserDatabase;
using FireBrowserExceptions;
using FireBrowserFavorites;
using Microsoft.UI.Xaml;

using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
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
        [ObservableProperty]
        public bool _isFavoriteCardEnabled = true;
        [ObservableProperty]
        public bool _isFavoriteExpanded;
        [ObservableProperty]
        public bool _isHistoryCardEnabled = true;
        [ObservableProperty]
        public bool _isHistoryExpanded;

        private DispatcherTimer timer { get; set; }

        public ObservableCollection<HistoryItem> HistoryItems { get; set; }
        public ObservableCollection<FavItem> FavoriteItems { get; set; }
        private void UpdateUIControls()
        {
            FireBrowserMultiCore.Settings userSettings = FireBrowserMultiCore.UserFolderManager.LoadUserSettings(FireBrowserMultiCore.AuthService.CurrentUser);
            NtpTimeEnabled = userSettings.NtpDateTime == "1";
            IsFavoriteExpanded = userSettings.IsFavoritesToggled == "1";
            IsHistoryExpanded = userSettings.IsHistoryToggled == "1";
            IsNtpTimeVisible = NtpTimeEnabled;
            NtpCoreVisibility = IsNtpTimeVisible ? Visibility.Visible : Visibility.Collapsed;
            OnPropertyChanged(nameof(NtpCoreVisibility));
            OnPropertyChanged(nameof(IsNtpTimeVisible));
            OnPropertyChanged(nameof(NtpTimeEnabled));
            OnPropertyChanged(nameof(IsFavoriteExpanded));
            OnPropertyChanged(nameof(IsHistoryExpanded));
            userSettings.NtpDateTime = NtpTimeEnabled ? "1" : "0";
            FireBrowserMultiCore.UserFolderManager.SaveUserSettings(FireBrowserMultiCore.AuthService.CurrentUser, userSettings);

        }
        public void RaisePropertyChanges([CallerMemberName] string? propertyName = null)
        {
            OnPropertyChanged(propertyName);
        }

        public ObservableCollection<FavItem> LoadFavorites()
        {
            ObservableCollection<FavItem> favorites = new ObservableCollection<FavItem>();

            try
            {
                FavManager fs = new FavManager();
                favorites = fs.LoadFav(FireBrowserMultiCore.AuthService.CurrentUser).ToObservableCollection();
                return favorites.ToObservableCollection();
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return new ObservableCollection<FavItem>();
            }

        }


        public HomeViewModel()
        {
            // TODO: add bingSearchApi -> helpers BingSearchApi == read more about JObject and JToken to parse..  foreach() or linq(). 

            FavoriteItems = new ObservableCollection<FavItem>();
            FavoriteItems.CollectionChanged += (s, e) => OnPropertyChanged(nameof(FavoriteItems));

            HistoryItems = new ObservableCollection<HistoryItem>();
            HistoryItems.CollectionChanged += (s, e) => OnPropertyChanged(nameof(HistoryItems));

        }

        public Task Intialize()
        {

            UpdateUIControls();

            if (NtpTimeEnabled)
            {
                InitClock();
            }

            return Task.CompletedTask;

        }
        private void UpdateClock()
        {
            // call this to update ui if a user has turned on/off ntp to be (dis)enabled.  
            UpdateUIControls();
            (NtpTimeText, NtpDateText) = (DateTime.Now.ToString("H:mm"), $"{DateTime.Today.DayOfWeek}, {DateTime.Today.ToString("MMMM d")}");
            OnPropertyChanged(nameof(NtpTimeText));
            OnPropertyChanged(nameof(NtpDateText));

        }
        private void InitClock()
        {
            // intial time => use timer to update then after that.
            UpdateClock();
            timer = new DispatcherTimer();
            // let refresh every four seconds and allow ui to work.
            timer.Interval = new System.TimeSpan(0, 0, 4);
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