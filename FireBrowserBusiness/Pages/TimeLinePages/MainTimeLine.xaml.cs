using FireBrowserBusiness;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FireBrowserWinUi3.Pages.TimeLinePages;
public sealed partial class MainTimeLine : Page
{
    public MainTimeLine()
    {
        this.InitializeComponent();
    }

    private readonly List<(string Tag, Type Page)> _pages = new()
    {
         ("history", typeof(TimeLinePages.HistoryTimeLine)),
         ("apps", typeof(TimeLinePages.AppsTimeLine)),
         ("favorites", typeof(TimeLinePages.FavoritesTimeLine)),
         ("downloads", typeof(TimeLinePages.DownloadsTimeLine))
    };

    private void NavigationView_Loaded(object sender, RoutedEventArgs e)
    {
        ContentFrame.Navigated += On_Navigated;

        var window = (Application.Current as App)?.m_window as MainWindow;
        var urlBoxText = window.UrlBox.Text;
        var navigateTo = urlBoxText switch
        {
            string s when s.Contains("firebrowser://apps") => ("apps", NavigationView.MenuItems[0]),
            string s when s.Contains("firebrowser://history") => ("history", NavigationView.MenuItems[1]),
            string s when s.Contains("firebrowser://favorites") => ("favorites", NavigationView.MenuItems[2]),
            string s when s.Contains("firebrowser://downloads") => ("downloads", NavigationView.MenuItems[3]),
            _ => (null, null),// default case
        };
        if (navigateTo.Item1 != null && navigateTo.Item2 != null)
        {
            NavigationView.SelectedItem = navigateTo.Item2;
            NavigationView_Navigate(navigateTo.Item1, new Microsoft.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo());
        }
        else
        {
            NavigationView.SelectedItem = NavigationView.MenuItems[0];
            NavigationView_Navigate("apps", new Microsoft.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo());
        } // Default behavior
    }

    private void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        if (args.IsSettingsInvoked == true)
        {
            NavigationView_Navigate("history", args.RecommendedNavigationTransitionInfo);
        }
        else if (args.InvokedItemContainer != null)
        {
            var navItemTag = args.InvokedItemContainer.Tag.ToString();
            NavigationView_Navigate(navItemTag, args.RecommendedNavigationTransitionInfo);
        }
    }

    private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.IsSettingsSelected == true)
        {
            NavigationView_Navigate("history", args.RecommendedNavigationTransitionInfo);
        }
        else if (args.SelectedItemContainer != null)
        {
            var navItemTag = args.SelectedItemContainer.Tag.ToString();
            NavigationView_Navigate(navItemTag, args.RecommendedNavigationTransitionInfo);
            var window = (Application.Current as App)?.m_window as MainWindow;
            window.UrlBox.Text = "firebrowser://" + navItemTag.ToString();
        }
    }

    private void NavigationView_Navigate(
      string navItemTag,
      Microsoft.UI.Xaml.Media.Animation.NavigationTransitionInfo transitionInfo)
    {
        Type _page = null;
        if (navItemTag == "apps")
        {
            _page = typeof(AppsTimeLine);
        }
        else
        {
            var item = _pages.FirstOrDefault(p => p.Tag.Equals(navItemTag));
            _page = item.Page;
        }
        // Get the page type before navigation so you can prevent duplicate
        // entries in the backstack.
        var preNavPageType = ContentFrame.CurrentSourcePageType;

        // Only navigate if the selected page isn't currently loaded.
        if (!(_page is null) && !Type.Equals(preNavPageType, _page))
        {
            ContentFrame.Navigate(_page, transitionInfo);
        }
    }

    private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
    {
        throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
    }

    private bool TryGoBack()
    {
        if (!ContentFrame.CanGoBack)
            return false;

        // Don't go back if the nav pane is overlayed.
        if (NavigationView.IsPaneOpen &&
            (NavigationView.DisplayMode == NavigationViewDisplayMode.Compact ||
             NavigationView.DisplayMode == NavigationViewDisplayMode.Minimal))
            return false;

        ContentFrame.GoBack();
        return true;
    }

    private void On_Navigated(object sender, NavigationEventArgs e)
    {
        NavigationView.IsBackEnabled = ContentFrame.CanGoBack;

        if (ContentFrame.SourcePageType != null)
        {
            var item = _pages.FirstOrDefault(p => p.Page == e.SourcePageType);
        }
    }
}