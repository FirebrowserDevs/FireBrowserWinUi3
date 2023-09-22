using System;

namespace FireBrowserMultiCore;
public class User
{
    public bool IsFirstLaunch { get; set; }
    public string Username { get; set; }
    public Guid UserId { get; set; }
    public string DirectoryPath { get; set; } // Changed from UserPath to DirectoryPath
    public string BrowserPath { get; set; }

    public string DataBasePath { get; set; }

    public Settings BrowserSettings { get; set; }
}