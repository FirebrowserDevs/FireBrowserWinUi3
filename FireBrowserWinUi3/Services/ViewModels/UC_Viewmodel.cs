﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FireBrowserWinUi3.Pages.Patch;
using FireBrowserWinUi3.Services;
using FireBrowserWinUi3.Services.Models;
using FireBrowserWinUi3Core.Helpers;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using WinRT.Interop;

namespace FireBrowserWinUi3
{
    public partial class UC_Viewmodel : ObservableRecipient
    {
        public UC_Viewmodel() => Users = new List<UserExtend>();
        public List<UserExtend> Users { get; set; }
        public UserExtend User { get; set; }
        public Window ParentWindow { get; set; }
        public UIElement ParentGrid { get; set; }

        private Func<bool> _IsCoreFolder = () =>
        {
            // Your condition here
            string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string backupDirectory = Path.Combine(documentsFolder);

            if (Directory.Exists(backupDirectory))
            {
                var backupFiles = Directory.GetFiles(backupDirectory, "*.firebackup");
                if (backupFiles.Length > 0)
                    return true;
                else
                    return false;
            }
            return false;
        };

        public bool IsCoreFolder { get { return _IsCoreFolder(); } }


        private ICommand _exitWindow;
        public ICommand ExitWindow => _exitWindow ??= new RelayCommand(() =>
        {
            AppService.IsAppGoingToClose = true;
            ParentWindow?.Close();

        });

        [RelayCommand]
        private void AdminCenter()
        {

            var win = new UpLoadBackup();
            win.AppWindow.SetPresenter(Microsoft.UI.Windowing.AppWindowPresenterKind.CompactOverlay);
            Windowing.DialogWindow(win);
            win.Activate();

        }

        [RelayCommand]
        private void OpenWindowsWeather()
        {
            var options = new Windows.System.LauncherOptions();
            options.DesiredRemainingView = Windows.UI.ViewManagement.ViewSizePreference.UseMinimum;

            // Launch the URI
            Windows.System.Launcher.LaunchUriAsync(new("msnweather://forecast"), options).GetAwaiter().GetResult();
        }

        [RelayCommand]
        private async Task BackUpCore()
        {

            BackUpDialog dlg = new BackUpDialog();
            dlg.XamlRoot = ParentGrid?.XamlRoot;
            await dlg.ShowAsync();

        }

        [RelayCommand(CanExecute = nameof(IsCoreFolder))]
        private async Task RestoreCore()
        {
            //usercentral is big enough now. 
            RestoreBackupDialog dlg = new RestoreBackupDialog();
            dlg.XamlRoot = ParentGrid?.XamlRoot;
            await dlg.ShowAsync();
        }

        public void RaisePropertyChanges([CallerMemberName] string propertyName = null) => OnPropertyChanged(propertyName);
    }
}