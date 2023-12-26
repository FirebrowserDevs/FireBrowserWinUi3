using FireBrowserBusinessCore.Models;
using FireBrowserSecureConnect.Controls;
using FireBrowserSecureConnect.ViewModels;
using Microsoft.UI.Xaml;
using SecureConnectOtp;
using System;
using System.Collections.ObjectModel;

namespace FireBrowserSecureConnect
{
    public static class TwoFactorsAuthentification
    {
        public static XamlRoot XamlRoot { get; set; }

        private static DispatcherTimer loginTimer;
        private static bool userAuthenticated = false;

        internal static ObservableCollection<TwoFactAuth> Items { get; } = new ObservableCollection<TwoFactAuth>();

        public static void Init()
        {
            loginTimer = new DispatcherTimer();
            loginTimer.Interval = TimeSpan.FromMinutes(5);
            loginTimer.Tick += (_, _) => userAuthenticated = false;

            InitializeData();
        }

        private static async void InitializeData()
        {
            var items = await FireBrowserBusinessCore.Helpers.TwoFactorsAuthentification.Load();

            foreach (var item in items)
            {
                var twoFactAuth = new TwoFactAuth(item);
                twoFactAuth.Start();
                Items.Add(twoFactAuth);
            }
        }

        public static async void ShowFlyout(FrameworkElement element)
        {
            HandleUserAuthentication();
            ResetTimerAndShowFlyout(element);
        }

        private static void HandleUserAuthentication()
        {
            userAuthenticated = !userAuthenticated;
        }

        private static void ResetTimerAndShowFlyout(FrameworkElement element)
        {
            loginTimer.Stop();
            loginTimer.Start();

            var flyout = new Two2FAFlyout();
            flyout.ShowAt(element);
        }

        public static void Add(string name, string secret)
        {
            var item = new TwoFactorAuthItem
            {
                Name = name,
                Secret = Base32Encoding.ToBytes(secret)
            };

            var twoFactAuth = new TwoFactAuth(item);
            twoFactAuth.Start();

            Items.Add(twoFactAuth);
            FireBrowserBusinessCore.Helpers.TwoFactorsAuthentification.Items.Add(item);
        }
    }
}