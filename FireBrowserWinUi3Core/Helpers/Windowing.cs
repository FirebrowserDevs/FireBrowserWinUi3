using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Display;
using Windows.Devices.Enumeration;
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

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsWindowEnabled(IntPtr hWnd);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

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

    public static void Center(IntPtr hWnd)
    {
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

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
    public static bool EnumTheWindows(IntPtr hWnd, IntPtr lParam)
    {
        StringBuilder sb = new StringBuilder(256);
        GetWindowText(hWnd, sb, sb.Capacity);
        Console.WriteLine(sb.ToString());
        return true;
    }
    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ShowWindow(IntPtr hWnd, WindowShowStyle nCmdShow);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    public static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
    public const uint SWP_NOSIZE = 0x0001;
    public const uint SWP_NOACTIVATE = 0x0010;

    // Full enum def is at https://github.com/dotnet/pinvoke/blob/main/src/User32/User32+WindowShowStyle.cs
    public enum WindowShowStyle : uint
    {
        SW_HIDE = 0,
        /// <summary>Activates and displays a window. If the window is minimized
        /// or maximized, the system restores it to its original size and
        /// position. An application should specify this flag when displaying
        /// the window for the first time.</summary>
        SW_SHOWNORMAL = 1,

        /// <summary>Activates the window and displays it as a minimized window.</summary>
        SW_SHOWMINIMIZED = 2,

        /// <summary>Activates the window and displays it as a maximized window.</summary>
        SW_SHOWMAXIMIZED = 3,

        /// <summary>Maximizes the specified window.</summary>
        SW_MAXIMIZE = 3,

        /// <summary>Displays a window in its most recent size and position.
        /// This value is similar to "ShowNormal", except the window is not
        /// actived.</summary>
        SW_SHOWNOACTIVATE = 4,

        /// <summary>Activates the window and displays it in its current size
        /// and position.</summary>
        SW_SHOW = 5,

        /// <summary>Minimizes the specified window and activates the next
        /// top-level window in the Z order.</summary>
        SW_MINIMIZE = 6,

        /// <summary>Displays the window as a minimized window. This value is
        /// similar to "ShowMinimized", except the window is not activated.</summary>
        SW_SHOWMINNOACTIVE = 7,

        /// <summary>Displays the window in its current size and position. This
        /// value is similar to "Show", except the window is not activated.</summary>
        SW_SHOWNA = 8,

        /// <summary>Activates and displays the window. If the window is
        /// minimized or maximized, the system restores it to its original size
        /// and position. An application should specify this flag when restoring
        /// a minimized window.</summary>
        SW_RESTORE = 9,

        /// <summary>Sets the show state based on the SW_ value specified in the
        /// STARTUPINFO structure passed to the CreateProcess function by the
        /// program that started the application.</summary>
        SW_SHOWDEFAULT = 10,

        /// <summary>Windows 2000/XP: Minimizes a window, even if the thread
        /// that owns the window is hung. This flag should only be used when
        /// minimizing windows from a different thread.</summary>
        SW_FORCEMINIMIZE = 11,
    }

    public static void HideWindow(IntPtr hWnd)
    {
        ShowWindow(hWnd, WindowShowStyle.SW_HIDE);
    }
    public static void MaximizeWindow(IntPtr hWnd)
    {
        ShowWindow(hWnd, WindowShowStyle.SW_MAXIMIZE);
    }

    static public async Task<SizeInt32?> SizeWindow()
    {
        var displayList = await DeviceInformation.FindAllAsync
                          (DisplayMonitor.GetDeviceSelector());

        if (!displayList.Any())
            return null;

        var monitorInfo = await DisplayMonitor.FromInterfaceIdAsync(displayList[0].Id);

        var winSize = new SizeInt32();

        if (monitorInfo == null)
        {
            winSize.Width = 800;
            winSize.Height = 1200;
        }
        else
        {
            winSize.Height = monitorInfo.NativeResolutionInRawPixels.Height;
            winSize.Width = monitorInfo.NativeResolutionInRawPixels.Width;
        }

        return winSize;
    }

    static public AppWindow GetAppWindow(Window window)
    {
        IntPtr hWnd = WindowNative.GetWindowHandle(window);
        WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
        return AppWindow.GetFromWindowId(wndId);
    }

    // get dpi for an element

}