using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;

namespace FireBrowserWinUi3;

public sealed partial class SetupWindow : Window
{
    private AppWindow appWindow;
    private AppWindowTitleBar titleBar;

    public SetupWindow()
    {
        this.InitializeComponent();

        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

        WindowId windowId = Win32Interop.GetWindowIdFromWindow(hWnd);

        appWindow = AppWindow.GetFromWindowId(windowId);
        appWindow.SetIcon("LogoSetup.ico");

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
    }

    private void setup_Loaded(object sender, RoutedEventArgs e)
    {
        setup.Navigate(typeof(FireBrowserSetup.SetupInit));
    }
}