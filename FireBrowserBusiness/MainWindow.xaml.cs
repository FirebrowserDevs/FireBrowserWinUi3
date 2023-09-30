using FireBrowserBusiness.Controls;
using FireBrowserBusiness.Pages;
using FireBrowserMultiCore;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinRT.Interop;
using Windowing = FireBrowserBusinessCore.Helpers.Windowing;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FireBrowserBusiness
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private AppWindow appWindow;
        private AppWindowTitleBar titleBar;
      
        public MainWindow()
        {
            InitializeComponent();

            Title();
            LoadUserDataAndSettings();              
            Launch();
        }

        public void Title()
        {
            // Make this maybe threaded class for faster handeling
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

            WindowId windowId = Win32Interop.GetWindowIdFromWindow(hWnd);

            appWindow = AppWindow.GetFromWindowId(windowId);
            appWindow.SetIcon("Logo.ico");

            if (!AppWindowTitleBar.IsCustomizationSupported())
            {
                // Why? Because I don't care
                throw new Exception("Unsupported OS version.");
            }
            else
            {
                titleBar = appWindow.TitleBar;
                titleBar.ExtendsContentIntoTitleBar = true;
                var btnColor = Colors.Transparent;
                titleBar.BackgroundColor = btnColor;
                titleBar.ButtonBackgroundColor = btnColor;
                titleBar.InactiveBackgroundColor = btnColor;
                titleBar.ButtonInactiveBackgroundColor = btnColor;
                titleBar.ButtonHoverBackgroundColor = btnColor;
            }

            buttons();
        }

        Settings userSettings = UserFolderManager.LoadUserSettings(AuthService.CurrentUser);
        private void LoadUserDataAndSettings()
        {
            if (GetUser() is not { } currentUser)
            {
                UserName.Text = Prof.Text = "DefaultUser";
                return;
            }

            if (!AuthService.IsUserAuthenticated && !AuthService.Authenticate(currentUser.Username))
            {
                return;
            }

            UserName.Text = Prof.Text = AuthService.CurrentUser?.Username ?? "DefaultUser";
        }

        public void buttons()
        {
            SetVisibility(AdBlock, userSettings.AdblockBtn != "0");
            SetVisibility(ReadBtn, userSettings.ReadButton != "0");
            SetVisibility(BtnTrans, userSettings.Translate != "0");
            SetVisibility(BtnDark, userSettings.DarkIcon != "0");
            SetVisibility(ToolBoxMore, userSettings.ToolIcon != "0");
            SetVisibility(AddFav, userSettings.FavoritesL != "0");
            SetVisibility(FavoritesButton, userSettings.Favorites != "0");
            SetVisibility(DownBtn, userSettings.Downloads != "0");
            SetVisibility(History, userSettings.Historybtn != "0");
            SetVisibility(QrBtn, userSettings.QrCode != "0");
        }

        private void SetVisibility(UIElement element, bool isVisible)
        {
            element.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
        }


        private FireBrowserMultiCore.User GetUser()
        {
            // Check if the user is authenticated.
            if (AuthService.IsUserAuthenticated)
            {
                // Return the authenticated user.
                return AuthService.CurrentUser;
            }

            // If no user is authenticated, return null or handle as needed.
            return null;
        }


        private void Launch()
        {
            Tabs.TabItems.Add(CreateNewTab(typeof(NewTab)));
        }

        private void TabView_AddTabButtonClick(TabView sender, object args)
        {
            sender.TabItems.Add(CreateNewTab());
        }

        #region TitleBar

        private AppWindow GetAppWindowForCurrentWindow()
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            return AppWindow.GetFromWindowId(wndId);
        }    

        #endregion

        public class Passer
        {
            public FireBrowserTabViewItem Tab { get; set; }
            public FireBrowserTabView TabView { get; set; }
            public object Param { get; set; }
            public string UserName { get; set; }
        }

        private FireBrowserTabViewItem CreateNewTab(Type page = null, object param = null, int index = -1)
        {
            if (index == -1) index = Tabs.TabItems.Count;

            var newItem = new FireBrowserTabViewItem
            {
                Header = "FireBrowser HomePage",
                IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource { Symbol = Symbol.Home },
                Style = (Style)Application.Current.Resources["FloatingTabViewItemStyle"]
            };

            var passer = new Passer { Tab = newItem, TabView = Tabs, Param = param };
           
            double Margin = 0;
            Margin = ClassicToolbar.Height;
            Frame frame = new()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Thickness(0, Margin, 0, 0)
            };

            if (page != null)
            {
                frame.Navigate(page, passer);
            }
            else
            {
                frame.Navigate(typeof(Pages.NewTab), passer);
            }

            var toolTip = new ToolTip();
            var grid = new Grid();
            var previewImage = new Image();
            var textBlock = new TextBlock();

            grid.Children.Add(previewImage);
            grid.Children.Add(textBlock);
            toolTip.Content = grid;
            ToolTipService.SetToolTip(newItem, toolTip);

            newItem.Content = frame;
            return newItem;
        }

        public Frame TabContent
        {
            get
            {
                FireBrowserTabViewItem selectedItem = (FireBrowserTabViewItem)Tabs.SelectedItem;
                if (selectedItem != null)
                {
                    return (Frame)selectedItem.Content;
                }
                return null;
            }
        }


        [DllImport("Shcore.dll", SetLastError = true)]
        public static extern int GetDpiForMonitor(IntPtr hmonitor, Windowing.Monitor_DPI_Type dpiType, out uint dpiX, out uint dpiY);


        private double GetScaleAdjustment()
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            DisplayArea displayArea = DisplayArea.GetFromWindowId(wndId, DisplayAreaFallback.Primary);
            IntPtr hMonitor = Win32Interop.GetMonitorFromDisplayId(displayArea.DisplayId);
            int result = GetDpiForMonitor(hMonitor, Windowing.Monitor_DPI_Type.MDT_Default_DPI, out uint dpiX, out uint _);
            if (result != 0)
            {
                throw new Exception("Could Not Get Dpi");
            }
            uint scaleFactorProcent = (uint)(((long)dpiX * 100 + (96 >> 1)) / 96);
            return scaleFactorProcent / 100.0;
        }


        private void Tabs_Loaded(object sender, RoutedEventArgs e)
        {
            Apptitlebar.SizeChanged += Apptitlebar_SizeChanged;
        }

        private void Apptitlebar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double scaleAdjustment = GetScaleAdjustment();
            Apptitlebar.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            var customDragRegionPosition = Apptitlebar.TransformToVisual(null).TransformPoint(new Point(0, 0));

            var dragRects = new Windows.Graphics.RectInt32[2];

            for (int i = 0; i < 2; i++)
            {
                dragRects[i] = new Windows.Graphics.RectInt32
                {
                    X = (int)((customDragRegionPosition.X + (i * Apptitlebar.ActualWidth / 2)) * scaleAdjustment),
                    Y = (int)(customDragRegionPosition.Y * scaleAdjustment),
                    Height = (int)((Apptitlebar.ActualHeight - customDragRegionPosition.Y) * scaleAdjustment),
                    Width = (int)((Apptitlebar.ActualWidth / 2) * scaleAdjustment)
                };
            }

            appWindow.TitleBar?.SetDragRectangles(dragRects);
        }

        private void Apptitlebar_LayoutUpdated(object sender, object e)
        {
            double scaleAdjustment = GetScaleAdjustment();
            Apptitlebar.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            var customDragRegionPosition = Apptitlebar.TransformToVisual(null).TransformPoint(new Point(0, 0));

            var dragRectsList = new List<Windows.Graphics.RectInt32>();

            for (int i = 0; i < 2; i++)
            {
                var dragRect = new Windows.Graphics.RectInt32
                {
                    X = (int)((customDragRegionPosition.X + (i * Apptitlebar.ActualWidth / 2)) * scaleAdjustment),
                    Y = (int)(customDragRegionPosition.Y * scaleAdjustment),
                    Height = (int)((Apptitlebar.ActualHeight - customDragRegionPosition.Y) * scaleAdjustment),
                    Width = (int)((Apptitlebar.ActualWidth / 2) * scaleAdjustment)
                };

                dragRectsList.Add(dragRect);
            }

            var dragRects = dragRectsList.ToArray();

            if (appWindow.TitleBar != null)
            {
                appWindow.TitleBar.SetDragRectangles(dragRects);
            }
        }

        private int maxTabItems = 20;
        private void Tabs_TabItemsChanged(TabView sender, IVectorChangedEventArgs args)
        {
            if (Tabs.TabItems.Count < maxTabItems)
            {
                Tabs.IsAddTabButtonVisible = true;
            }
            else
            {
                Tabs.IsAddTabButtonVisible = false;
            }
        }
    }
}
