using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.Runtime.InteropServices;
using Windows.Graphics;
using WinRT.Interop;

namespace FireBrowserWinUi3Core.Helpers;
public class Windowing
{
    public enum Monitor_DPI_Type : int
    {
        MDT_Effective_DPI = 0,
        MDT_Angular_DPI = 1,
        MDT_Raw_DPI = 2,
        MDT_Default_DPI = MDT_Effective_DPI,
    }

    [DllImport("Shcore.dll", SetLastError = true)]
    public static extern int GetDpiForMonitor(IntPtr hmonitor, Windowing.Monitor_DPI_Type dpiType, out uint dpiX, out uint dpiY);

    public static void Center(Window window)
    {
        IntPtr hWnd = WindowNative.GetWindowHandle(window);
        WindowId windowId = Win32Interop.GetWindowIdFromWindow(hWnd);

        if (AppWindow.GetFromWindowId(windowId) is AppWindow appWindow &&
            DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Nearest) is DisplayArea displayArea)
        {
            PointInt32 CenteredPosition = appWindow.Position;
            CenteredPosition.X = (displayArea.WorkArea.Width - appWindow.Size.Width) / 2;
            CenteredPosition.Y = (displayArea.WorkArea.Height - appWindow.Size.Height) / 2;
            appWindow.Move(CenteredPosition);
        }
    }
}