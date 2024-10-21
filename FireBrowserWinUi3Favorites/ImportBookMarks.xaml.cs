using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FireBrowserWinUi3Favorites
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ImportBookMarks : ContentDialog
    {
        private FavManager _favManager;

        public ImportBookMarks()
        {
            this.InitializeComponent();
            _favManager = new FavManager();
        }

        private async void ChromeButton_Click(object sender, RoutedEventArgs e)
        {
            await ImportBookmarksAsync("Chrome");
        }

        private async void FirefoxButton_Click(object sender, RoutedEventArgs e)
        {
            await ImportBookmarksAsync("Firefox");
        }

        private async void EdgeButton_Click(object sender, RoutedEventArgs e)
        {
            await ImportBookmarksAsync("Edge");
        }

        private async void ArcButton_Click(object sender, RoutedEventArgs e)
        {
            await ImportBookmarksAsync("Arc");
        }

        private async void BraveButton_Click(object sender, RoutedEventArgs e)
        {
            await ImportBookmarksAsync("Brave");
        }


        private async Task ImportBookmarksAsync(string browserName)
        {
            ImportProgressBar.Visibility = Visibility.Visible;
            StatusTextBlock.Text = $"Importing bookmarks from {browserName}...";

            try
            {
                await Task.Run(() => _favManager.ImportFavoritesFromOtherBrowsers(browserName));
                StatusTextBlock.Text = $"Successfully imported bookmarks from {browserName}.";
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"Error importing bookmarks from {browserName}: {ex.Message}";
            }
            finally
            {
                ImportProgressBar.Visibility = Visibility.Collapsed;
            }
        }
    }
}
