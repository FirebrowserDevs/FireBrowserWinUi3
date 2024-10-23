using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NewHomePage;
using System;
using System.Collections.ObjectModel;

namespace NewHomePage
{
    public sealed partial class SettingsOverviewPage : Page
    {
        public ObservableCollection<SettingItem> QuickActions { get; } = new ObservableCollection<SettingItem>();
        public ObservableCollection<SettingItem> MainSettings { get; } = new ObservableCollection<SettingItem>();

        public string UserName { get; private set; }
        public string UserEmail { get; private set; }
        public double StorageUsed { get; private set; }
        public double StorageTotal { get; private set; }
        public string UserFolderPath { get; private set; }

        public SettingsOverviewPage()
        {
            this.InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            // Populate Quick Actions
            QuickActions.Add(new SettingItem { Name = "Clear Browsing Data", Icon = "\uE74D" });
            QuickActions.Add(new SettingItem { Name = "Manage Passwords", Icon = "\uE8D7" });
            QuickActions.Add(new SettingItem { Name = "Privacy Settings", Icon = "\uE72E" });
            QuickActions.Add(new SettingItem { Name = "Customize Theme", Icon = "\uE771" });

            MainSettings.Add(new SettingItem { Name = "General", Description = "Default browser, startup, tabs", Icon = "\uE713", NavigationPage = typeof(GeneralSettingsPage) });
            MainSettings.Add(new SettingItem { Name = "Privacy and Security", Description = "Cookies, site data, safe browsing", Icon = "\uE72E", NavigationPage = typeof(PrivacySettingsPage) });
            MainSettings.Add(new SettingItem { Name = "Appearance", Description = "Theme, font size, home button", Icon = "\uE771", NavigationPage = typeof(AppearanceSettingsPage) });
            MainSettings.Add(new SettingItem { Name = "Search Engine", Description = "Manage search engines", Icon = "\uE721", NavigationPage = typeof(SearchEngineSettingsPage) });
            MainSettings.Add(new SettingItem { Name = "Extensions", Description = "Manage browser extensions", Icon = "\uE710", NavigationPage = typeof(ExtensionsSettingsPage) });
            MainSettings.Add(new SettingItem { Name = "Downloads", Description = "Download location, prompts", Icon = "\uE896", NavigationPage = typeof(DownloadsSettingsPage) });
        

            // Load user information (replace with actual data retrieval logic)
            UserName = "John Doe";
            UserEmail = "johndoe@example.com";
            StorageUsed = 6.5;
            StorageTotal = 10;
            UserFolderPath = @"C:\Users\JohnDoe";
        }

        private void NavigateToSetting(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var setting = (SettingItem)button.DataContext;

            if (setting.NavigationPage != null)
            {
                Frame.Navigate(setting.NavigationPage);
            }
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            ProfileFlyout.ShowAt(ProfileButton);
        }

        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            // Implement logout functionality here
            ProfileFlyout.Hide();
            // For example: Frame.Navigate(typeof(LoginPage));
        }
    }

    public class SettingItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public Type NavigationPage { get; set; }
    }
}