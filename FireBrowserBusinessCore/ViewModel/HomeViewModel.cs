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
        private bool _isFavoriteCardEnabled = true;
        [ObservableProperty]
        private bool _isFavoriteExpanded;
        [ObservableProperty]
        private bool _isHistoryCardEnabled = true;
        [ObservableProperty]
        private bool _isHistoryExpanded;
        [ObservableProperty]
        private bool _isSearchBoxEnabled = true;
        [ObservableProperty]
        private Visibility _isFavoritesVisible;
        [ObservableProperty]
        private Visibility _isHistoryVisible;
        [ObservableProperty]
        private Visibility _IsSearchVisible;


        public delegate void DelegateSaveSettings(FireBrowserMultiCore.User user, FireBrowserMultiCore.Settings settings);
        public DelegateSaveSettings SaveSettings { get; set; }

        public FireBrowserMultiCore.Settings CoreSettings { get; set; }
        private DispatcherTimer timer { get; set; }

        public ObservableCollection<HistoryItem> HistoryItems { get; set; }
        public ObservableCollection<FavItem> FavoriteItems { get; set; }
        private void UpdateUIControls()
        {
            // use CoreSettings to save file access -> to Settings.json every 4 seconds handle in one place usings delegate...


            IsFavoritesVisible = CoreSettings.IsFavoritesVisible == "1" ? Visibility.Visible : Visibility.Collapsed;
            CoreSettings.IsFavoritesVisible = IsFavoritesVisible == Visibility.Visible ? "1" : "0";

            IsHistoryVisible = CoreSettings.IsHistoryVisible == "1" ? Visibility.Visible : Visibility.Collapsed;
            CoreSettings.IsHistoryVisible = IsHistoryVisible == Visibility.Visible ? "1" : "0";

            IsSearchVisible = CoreSettings.IsSearchVisible == "1" ? Visibility.Visible : Visibility.Collapsed;
            CoreSettings.IsSearchVisible = IsSearchVisible == Visibility.Visible ? "1" : "0";

            NtpCoreVisibility = IsNtpTimeVisible ? Visibility.Visible : Visibility.Collapsed;
            CoreSettings.NtpCoreVisibility = NtpCoreVisibility == Visibility.Visible ? "1" : "0";

            NtpTimeEnabled = CoreSettings.NtpDateTime == "1";
            CoreSettings.NtpDateTime = NtpTimeEnabled ? "1" : "0";

            IsFavoriteExpanded = CoreSettings.IsFavoritesToggled == "1";
            CoreSettings.IsFavoritesToggled = IsFavoriteExpanded ? "1" : "0";

            IsHistoryExpanded = CoreSettings.IsHistoryToggled == "1";
            CoreSettings.IsHistoryToggled = IsHistoryExpanded ? "1" : "0";

            IsNtpTimeVisible = NtpTimeEnabled;

            OnPropertyChanged(nameof(IsFavoritesVisible));
            OnPropertyChanged(nameof(IsHistoryVisible));
            OnPropertyChanged(nameof(IsSearchVisible));
            OnPropertyChanged(nameof(NtpTimeEnabled));
            OnPropertyChanged(nameof(IsFavoriteExpanded));
            OnPropertyChanged(nameof(IsHistoryExpanded));
            CoreSettings.NtpDateTime = NtpTimeEnabled ? "1" : "0";
            SaveSettings(FireBrowserMultiCore.AuthService.CurrentUser, CoreSettings);

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

            if (FireBrowserMultiCore.AuthService.IsUserAuthenticated)
                CoreSettings = FireBrowserMultiCore.UserFolderManager.LoadUserSettings(FireBrowserMultiCore.AuthService.CurrentUser);


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