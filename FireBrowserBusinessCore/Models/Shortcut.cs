using IWshRuntimeLibrary;
using System;
using System.IO;

namespace FireBrowserBusinessCore.Models;

public class Shortcut
{
    public async void CreateShortcut(string username)
    {
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        string shortcutPath = Path.Combine(desktopPath, $"{username}.lnk");

        var shell = new WshShell();
        var shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);

        // Set the target application and command-line argument
        shortcut.TargetPath = $"firebrowseruser://{username}"; // Include -user in the protocol

        // Set the description to include the desired properties
        shortcut.Description = $"-fireuser {username}"; // Set the description as needed

        // Set the icon for the shortcut to the path of the PNG file
        string iconPath = "ms-appx://Logo.ico";
        shortcut.IconLocation = iconPath;

        // Save the shortcut file
        shortcut.Save();
    }
}