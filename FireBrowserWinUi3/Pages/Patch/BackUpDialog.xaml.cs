using System;
using System.IO;
using FireBrowserWinUi3.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;  // WebView2 namespace

namespace FireBrowserWinUi3.Pages.Patch
{
    public sealed partial class BackUpDialog : ContentDialog
    {
        private const int MaxBackupCountStandard = 6;
        private const int MaxBackupCountPremium = 50;
        public bool IsBackupAllowed { get; set; } = true;
        public bool IsPremiumUser { get; set; } = false;
        private string premiumLicensePath;

        public BackUpDialog()
        {
            this.InitializeComponent();
            premiumLicensePath = Path.Combine(AppContext.BaseDirectory, "premium.license"); // Application startup path
            IsPremiumUser = CheckPremiumStatus();
            CheckBackupLimit();
        }

        // Method to check if the premium license file exists
        private bool CheckPremiumStatus()
        {
            // Check if the premium license file exists
            if (File.Exists(premiumLicensePath))
            {
                IsPremiumUser = true;
            }
            else
            {
                IsPremiumUser = false;
            }

            return IsPremiumUser;
        }

        private void CheckBackupLimit()
        {
            // Get the Documents folder path
            string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string backupDirectory = Path.Combine(documentsFolder);

            if (Directory.Exists(backupDirectory))
            {
                var backupFiles = Directory.GetFiles(backupDirectory, "*.firebackup");
                int maxBackupCount = IsPremiumUser ? MaxBackupCountPremium : MaxBackupCountStandard;

                if (backupFiles.Length >= maxBackupCount)
                {
                    IsBackupAllowed = false;
                    InfoBarBackupWarning.IsOpen = true;
                    DefaultInfo.IsOpen = false;
                    PrimaryButtonText = "Disabled"; // Change button text to indicate disabled
                }
                else
                {
                    IsBackupAllowed = true;
                    InfoBarBackupWarning.IsOpen = false;
                    DefaultInfo.IsOpen = true;
                }
            }
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (!IsBackupAllowed)
            {
                args.Cancel = true; // Prevent dialog close if max backups exist
                return;
            }

            // Backup creation logic
            string tempPath = Path.GetTempPath();
            string backupFilePath = Path.Combine(tempPath, "backup.fireback");
            using (FileStream fs = File.Create(backupFilePath)) ;

            Microsoft.Windows.AppLifecycle.AppInstance.Restart("");
        }


        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            PaymentWindow paymentWindow = new PaymentWindow();
            paymentWindow.Activate();
        }
    }
}
