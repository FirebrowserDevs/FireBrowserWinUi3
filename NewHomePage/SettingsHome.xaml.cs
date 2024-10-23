using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace NewHomePage
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsHome : Page
    {
        public ObservableCollection<SettingItem> QuickActions { get; } = new ObservableCollection<SettingItem>();
        public ObservableCollection<SettingItem> MainSettings { get; } = new ObservableCollection<SettingItem>();

        public SettingsHome()
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

            // Populate Main Settings
            MainSettings.Add(new SettingItem { Name = "General", Description = "Default browser, startup, tabs", Icon = "\uE713" });
            MainSettings.Add(new SettingItem { Name = "Privacy and Security", Description = "Cookies, site data, safe browsing", Icon = "\uE72E" });
            MainSettings.Add(new SettingItem { Name = "Appearance", Description = "Theme, font size, home button", Icon = "\uE771" });
            MainSettings.Add(new SettingItem { Name = "Search Engine", Description = "Manage search engines", Icon = "\uE721" });
            MainSettings.Add(new SettingItem { Name = "Extensions", Description = "Manage browser extensions", Icon = "\uE710" });
            MainSettings.Add(new SettingItem { Name = "Downloads", Description = "Download location, prompts", Icon = "\uE896" });
        }

        public class SettingItem
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string Icon { get; set; }
        }
    }
}
