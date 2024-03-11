using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using FireBrowserDatabase;
using FireBrowserWinUi3.Services;
using FireBrowserWinUi3.Services.Contracts;
using FireBrowserWinUi3Core.Helpers;
using FireBrowserWinUi3Core.Models;
using FireBrowserWinUi3Exceptions;
using FireBrowserWinUi3Favorites;
using Microsoft.UI.Xaml;
using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace FireBrowserWinUi3.ViewModels
{
    public partial class HomeViewModel : ObservableRecipient
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
        private bool _isFavoriteCardEnabled;
        [ObservableProperty]
        private bool _isFavoriteExpanded;
        [ObservableProperty]
        private bool _isHistoryCardEnabled;
        [ObservableProperty]
        private bool _isHistoryExpanded;
        [ObservableProperty]
        private bool _isSearchBoxEnabled;
        [ObservableProperty]
        private Visibility _isFavoritesVisible;
        [ObservableProperty]
        private Visibility _isHistoryVisible;
        [ObservableProperty]
        private Visibility _IsSearchVisible;

        public SettingsService SettingsService { get; set; }
private DispatcherTimer timer { get; set; }

        public ObservableCollection<HistoryItem> HistoryItems { get; set; }
        public ObservableCollection<FavItem> FavoriteItems { get; set; }
        private void LoadUISettings()
        {

            NtpCoreVisibility = SettingsService.CoreSettings.NtpCoreVisibility ? Visibility.Visible : Visibility.Collapsed;
            IsNtpTimeVisible = SettingsService.CoreSettings.NtpDateTime;
            NtpTimeEnabled = SettingsService.CoreSettings.NtpDateTime;

            IsFavoritesVisible = SettingsService.CoreSettings.IsFavoritesVisible ? Visibility.Visible : Visibility.Collapsed;
            IsFavoriteCardEnabled = SettingsService.CoreSettings.IsFavoritesVisible;
            IsHistoryVisible = SettingsService.CoreSettings.IsHistoryVisible ? Visibility.Visible : Visibility.Collapsed;
            IsHistoryCardEnabled = SettingsService.CoreSettings.IsHistoryVisible;
            IsSearchVisible = SettingsService.CoreSettings.IsSearchVisible ? Visibility.Visible : Visibility.Collapsed;
            IsSearchBoxEnabled = SettingsService.CoreSettings.IsSearchVisible;

            IsFavoriteExpanded = SettingsService.CoreSettings.IsFavoritesToggled;
            IsHistoryExpanded = SettingsService.CoreSettings.IsHistoryToggled;

            OnPropertyChanged(nameof(NtpCoreVisibility));
            OnPropertyChanged(nameof(IsNtpTimeVisible));
            OnPropertyChanged(nameof(NtpTimeEnabled));

            OnPropertyChanged(nameof(IsFavoritesVisible));
            OnPropertyChanged(nameof(IsHistoryVisible));
            OnPropertyChanged(nameof(IsSearchVisible));

            OnPropertyChanged(nameof(IsSearchBoxEnabled));
            OnPropertyChanged(nameof(IsFavoriteCardEnabled));
            OnPropertyChanged(nameof(IsHistoryCardEnabled));

            OnPropertyChanged(nameof(IsFavoriteExpanded));
            OnPropertyChanged(nameof(IsHistoryExpanded));



        }
        private async void UpdateUIControls()
        {
            /*
               - UI sets user settings to CoreSettings now apply property change settings back to UI. 
               - Get visible then convert to bool for settings. */

            NtpCoreVisibility = SettingsService.CoreSettings.NtpDateTime ? Visibility.Visible : Visibility.Collapsed;
            NtpTimeEnabled = SettingsService.CoreSettings.NtpDateTime;

            IsFavoriteCardEnabled = SettingsService.CoreSettings.IsFavoritesVisible;
            IsHistoryCardEnabled = SettingsService.CoreSettings.IsHistoryVisible;
            IsSearchBoxEnabled = SettingsService.CoreSettings.IsSearchVisible;

            IsFavoritesVisible = IsFavoriteCardEnabled ? Visibility.Visible : Visibility.Collapsed;
            IsHistoryVisible = IsHistoryCardEnabled ? Visibility.Visible : Visibility.Collapsed;
            IsSearchVisible = IsSearchBoxEnabled ? Visibility.Visible : Visibility.Collapsed;

            IsFavoriteExpanded = SettingsService.CoreSettings.IsFavoritesToggled;
            IsHistoryExpanded = SettingsService.CoreSettings.IsHistoryToggled;

            // - Use CoreSettings to save file access -> to Settings.json every 4 seconds handle in one place usings delegate...
            await SettingsService?.SaveChangesToSettings(FireBrowserWinUi3MultiCore.AuthService.CurrentUser, SettingsService.CoreSettings);

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
                favorites = fs.LoadFav(FireBrowserWinUi3MultiCore.AuthService.CurrentUser).ToObservableCollection();
                return favorites.ToObservableCollection();
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return new ObservableCollection<FavItem>();
            }

        }



        public HomeViewModel(IMessenger messenger)
            :base(messenger)  
        {
            // TODO: add bingSearchApi -> helpers BingSearchApi == read more about JObject and JToken to parse..  foreach() or linq(). 

            FavoriteItems = new ObservableCollection<FavItem>();
            FavoriteItems.CollectionChanged += (s, e) => OnPropertyChanged(nameof(FavoriteItems));

            HistoryItems = new ObservableCollection<HistoryItem>();
            HistoryItems.CollectionChanged += (s, e) => OnPropertyChanged(nameof(HistoryItems));

            // LOAD settings service 
            SettingsService = App.GetService<SettingsService>();
            SettingsService.Initialize(); 
            // load ui settings from CoreSettings. 
            LoadUISettings();

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
            // UpdateUIControls(); - no need to update controls here because it's fired through Initilaze from view when there is a UpdateUserSettings
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