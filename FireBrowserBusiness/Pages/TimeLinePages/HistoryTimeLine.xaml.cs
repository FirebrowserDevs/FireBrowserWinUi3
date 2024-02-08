using FireBrowserDatabase;
using FireBrowserDataCore.Actions;
using FireBrowserDataCore.Models;
using FireBrowserExceptions;
using FireBrowserMultiCore;
using Microsoft.Data.Sqlite;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using SQLitePCL;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FireBrowserWinUi3.Pages.TimeLinePages;
public sealed partial class HistoryTimeLine : Page
{
    public HistoryTimeLine()
    {
        this.InitializeComponent();
        FetchBrowserHistory();
    }

    public FireBrowserMultiCore.User user = AuthService.CurrentUser;
    private ObservableCollection<FireBrowserDatabase.HistoryItem> browserHistory;

    private async void FetchBrowserHistory()
    {

        try
        {
            HistoryActions historyActions = new HistoryActions(AuthService.CurrentUser.Username);
            var items = await historyActions.GetAllHistoryItems();
            BigTemp.ItemsSource = items; 
        }
        catch (Exception ex)
        {
            ExceptionLogger.LogException(ex);
        }
                
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        FetchBrowserHistory();
    }

    private void FilterBrowserHistory(string searchText)
    {
        if (browserHistory == null) return;

        // Clear the collection to start fresh with filtered items
        BigTemp.ItemsSource = null;

        // Filter the browser history based on the search text
        var filteredHistory = new ObservableCollection<FireBrowserDatabase.HistoryItem>(browserHistory
            .Where(item => item.Url.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                           item.Title?.Contains(searchText, StringComparison.OrdinalIgnoreCase) == true));

        // Bind the filtered browser history items to the ListView
        BigTemp.ItemsSource = filteredHistory;
    }
    private void Ts_TextChanged(object sender, TextChangedEventArgs e)
    {
        string searchText = Ts.Text;
        FilterBrowserHistory(searchText);
    }

    private async Task ClearDb()
    {
        HistoryActions historyActions = new HistoryActions(AuthService.CurrentUser.Username);
        await historyActions.DeleteAllHistoryItems();

        BigTemp.ItemsSource = null;
    }

    private async void Delete_Click(object sender, RoutedEventArgs e)
    {
        await ClearDb();
    }

    private string selectedHistoryItem;
    private void Grid_RightTapped(object sender, RightTappedRoutedEventArgs e)
    {
        // Get the selected HistoryItem object
        FireBrowserDatabase.HistoryItem historyItem = ((FrameworkElement)sender).DataContext as FireBrowserDatabase.HistoryItem;
        selectedHistoryItem = historyItem.Url;

        // Create a context menu flyout
        var flyout = new MenuFlyout();

        // Add a menu item for "Delete This Record" button
        var deleteMenuItem = new MenuFlyoutItem
        {
            Text = "Delete This Record",
        };

        // Set the icon for the menu item using the Unicode escape sequence
        deleteMenuItem.Icon = new FontIcon
        {
            Glyph = "\uE74D" // Replace this with the Unicode escape sequence for your desired icon
        };

        // Handle the click event directly within the right-tapped event handler
        deleteMenuItem.Click += async(s, args) =>
        {
            HistoryActions historyActions = new HistoryActions(AuthService.CurrentUser.Username);
            await historyActions.DeleteHistoryItem(selectedHistoryItem);

            if (BigTemp.ItemsSource is ObservableCollection<FireBrowserDatabase.HistoryItem> historyItems)
            {
                var itemToRemove = historyItems.FirstOrDefault(item => item.Url == selectedHistoryItem);
                if (itemToRemove != null)
                {
                    historyItems.Remove(itemToRemove);
                }
            }
            // After deletion, you may want to update the UI or any other actions
        };

        flyout.Items.Add(deleteMenuItem);

        // Show the context menu flyout
        flyout.ShowAt((FrameworkElement)sender, e.GetPosition((FrameworkElement)sender));
    }
}