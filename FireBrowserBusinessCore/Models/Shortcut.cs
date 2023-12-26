using IWshRuntimeLibrary;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FireBrowserBusinessCore.Models;

public class Shortcut
{
    public async Task CreateShortcut(string username)
    {
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        string shortcutPath = Path.Combine(desktopPath, $"{username}.lnk");

        var shell = new WshShell();
        IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);

        await Task.Run(() =>
        {
            // Set the target application and command-line argument
            shortcut.TargetPath = $"firebrowseruser://{username}"; // Include -user in the protocol

            // Set the description to include the desired properties
            shortcut.Description = $"-fireuser {username}"; // Set the description as needed

            // Set the icon for the shortcut to the path of the ICO file
            string iconPath = "ms-appx://Logo.ico";
            shortcut.IconLocation = iconPath;

            // Save the shortcut file
            shortcut.Save();
        });
    }
}