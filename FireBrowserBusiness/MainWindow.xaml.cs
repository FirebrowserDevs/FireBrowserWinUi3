using FireBrowserBusiness.Controls;
using FireBrowserBusiness.Pages;
using FireBrowserMultiCore;
using FireBrowserWinUi3.Setup;
using Microsoft.UI;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics;
using Windows.Media.Core;
using Windows.System;
using Windows.UI.ViewManagement;
using WinRT.Interop;

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
            }
          
            LoadUserDataAndSettings();
                  
            Launch();
        }

        private void LoadUserDataAndSettings()
        {
            FireBrowserMultiCore.User currentUser = GetUser();

            if (currentUser != null)
            {
                if (!AuthService.IsUserAuthenticated)
                {
                    bool isAuthenticated = AuthService.Authenticate(currentUser.Username);

                    if (!isAuthenticated)
                    {
                     
                        return;
                    }
                }

                Settings userSettings = UserFolderManager.LoadUserSettings(AuthService.CurrentUser);

                // Update the TextBlock with the username.
                UserName.Text = AuthService.CurrentUser.Username;
                Prof.Text = AuthService.CurrentUser.Username;
                // You can also update other UI elements with user-specific data/settings.
                // For example:
                // someOtherTextBlock.Text = userSettings.SomeProperty;
            }
            else
            {
                // No user selected or authenticated, use a default username.
                UserName.Text = "DefaultUser";
                Prof.Text = "DefaultUser";
            }
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

        [DllImport("Shcore.dll", SetLastError = true)]
        internal static extern int GetDpiForMonitor(IntPtr hmonitor, Monitor_DPI_Type dpiType, out uint dpiX, out uint dpiY);

        internal enum Monitor_DPI_Type : int
        {
            MDT_Effective_DPI = 0,
            MDT_Angular_DPI = 1,
            MDT_Raw_DPI = 2,
            MDT_Default_DPI = MDT_Effective_DPI,
        }

        private double GetScaleAdjustment()
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            DisplayArea displayArea = DisplayArea.GetFromWindowId(wndId, DisplayAreaFallback.Primary);
            IntPtr hMonitor = Win32Interop.GetMonitorFromDisplayId(displayArea.DisplayId);
            int result = GetDpiForMonitor(hMonitor, Monitor_DPI_Type.MDT_Default_DPI, out uint dpiX, out uint _);
            if(result != 0)
            {
                throw new Exception("Could Not Get Dpi");
            }
            uint scaleFactorProcent = (uint)(((long)dpiX * 100 + (96 >> 1)) / 96);
            return scaleFactorProcent / 100.0;
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
                // Disable the button or show a message indicating the limit is reached.
                // For example, if you have a button named AddTabItemButton:
                Tabs.IsAddTabButtonVisible = false;
            }
        }
    }
}
