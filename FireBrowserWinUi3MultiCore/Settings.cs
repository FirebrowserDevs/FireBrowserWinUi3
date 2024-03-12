using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FireBrowserWinUi3MultiCore;
public class Settings
{

    void DefaultSettings()
    {
        var self = this;
        self.PackageName = "FireBrowswerWinUi3";
        self.DisableJavaScript = false;
        self.DisablePassSave = false;
        self.DisableWebMess = false;
        self.DisableGenAutoFill = false;
        self.ColorBackground = "#000000";
        self.StatusBar = true;
        self.BrowserKeys = true;
        self.BrowserScripts = true;
        self.Useragent = "WebView";
        self.LightMode = false;
        self.OpSw = true;
        self.EngineFriendlyName = "Google";
        self.SearchUrl = "https://www.google.com/search?q=";
        self.ColorTool = "#000000";
        self.ColorTV = "#000000";
        self.Background = 0;
        self.Auto = false;
        self.Lang = "nl-NL";
        self.ReadButton = true;
        self.AdblockBtn = true;
        self.Downloads = true;
        self.Translate = true;
        self.Favorites = true;
        self.Historybtn = true;
        self.QrCode = true;
        self.FavoritesL = true;
        self.ToolIcon = true;
        self.DarkIcon = true;
        self.OpenTabHandel = false;
        self.NtpDateTime = true;
        self.ExitDialog = false;
        self.NtpTextColor = "#FFFFFF";
        self.ExceptionLog = "Low";
        self.Eq2fa = true;
        self.Eqfav = false;
        self.EqHis = false;
        self.Eqsets = false;
        self.TrackPrevention = 2;
        self.ResourceSave = false;
        self.ConfirmCloseDlg = true;
        self.IsHistoryToggled = false;
        self.IsFavoritesToggled = false;
        self.IsSearchBoxToggled = false;
        self.IsFavoritesVisible = true;
        self.IsHistoryVisible = true;
        self.IsSearchVisible = true;
        self.NtpCoreVisibility = true;

    }

    [JsonIgnore]
    public Settings Self { get; set; }

    public Settings(Settings settings)
    {
        Self = settings;
    }
    public Settings(bool LoadDefaults)
    {
        Self = this;

        if (LoadDefaults)
            DefaultSettings();

    }
    public Settings() { }

    [Key]
    [DefaultValue("FireBrowserWinUi3")]
    public string PackageName { get; set; }

    [DefaultValue(false)]
    public bool DisableJavaScript { get; set; } // Use "0" for false, "1" for true
    [DefaultValue(false)]
    public bool DisablePassSave { get; set; } // Use "0" for false, "1" for true
    [DefaultValue(false)]
    public bool DisableWebMess { get; set; } // Use "0" for false, "1" for true
    [DefaultValue(false)]
    public bool DisableGenAutoFill { get; set; } // Use "0" for false, "1" for true
    [DefaultValue("#000000")]
    public string ColorBackground { get; set; }
    [DefaultValue(true)]
    public bool StatusBar { get; set; } // Use "0" for false, "1" for true
    [DefaultValue(true)]
    public bool BrowserKeys { get; set; } // Use "0" for false, "1" for true
    [DefaultValue(true)]
    public bool BrowserScripts { get; set; } // Use "0" for false, "1" for true
    [DefaultValue("WebView")]
    public string Useragent { get; set; }
    [DefaultValue(false)]
    public bool LightMode { get; set; } // Use "0" for false, "1" for true
    [DefaultValue(true)]
    public bool OpSw { get; set; } // Use "0" for false, "1" for true
    [DefaultValue("Google")]
    public string EngineFriendlyName { get; set; }
    [DefaultValue("https://www.google.com/search?q=")]
    public string SearchUrl { get; set; }
    [DefaultValue("#000000")]
    public string ColorTool { get; set; }
    [DefaultValue("#000000")]
    public string ColorTV { get; set; }
    [DefaultValue(0)]
    public int Background { get; set; } // Use "0" for false, "1" for true
    [DefaultValue(false)]
    public bool Auto { get; set; } // Use "0" for false, "1" for true
    [DefaultValue("nl-NL")]
    public string Lang { get; set; }
    [DefaultValue(true)]
    public bool ReadButton { get; set; }
    [DefaultValue(true)]
    public bool AdblockBtn { get; set; }
    [DefaultValue(true)]
    public bool Downloads { get; set; }
    [DefaultValue(true)]
    public bool Translate { get; set; }
    [DefaultValue(true)]
    public bool Favorites { get; set; }
    [DefaultValue(true)]
    public bool Historybtn { get; set; }
    [DefaultValue(true)]
    public bool QrCode { get; set; }
    [DefaultValue(true)]
    public bool FavoritesL { get; set; }
    [DefaultValue(true)]
    public bool ToolIcon { get; set; }

    [DefaultValue(true)]
    public bool DarkIcon { get; set; }
    [DefaultValue(false)]
    public bool OpenTabHandel { get; set; }
    [DefaultValue(false)]
    public bool NtpDateTime { get; set; }

    [DefaultValue(false)]
    public bool ExitDialog { get; set; }
    [DefaultValue("#000000")]
    public string NtpTextColor { get; set; }
    [DefaultValue("Low")]
    public string ExceptionLog { get; set; }
    [DefaultValue(true)]
    public bool Eq2fa { get; set; }
    [DefaultValue(false)]
    public bool Eqfav { get; set; }
    [DefaultValue(false)]
    public bool EqHis { get; set; }
    [DefaultValue(false)]
    public bool Eqsets { get; set; }
    [DefaultValue("2")]

    public int TrackPrevention { get; set; }
    [DefaultValue(false)]
    public bool ResourceSave { get; set; }
    [DefaultValue(true)]
    public bool ConfirmCloseDlg { get; set; }
    [DefaultValue(false)]
    public bool IsFavoritesToggled { get; set; }
    [DefaultValue(false)]
    public bool IsSearchBoxToggled { get; set; }
    [DefaultValue(false)]
    public bool IsHistoryToggled { get; set; }
    [DefaultValue(true)]
    public bool IsHistoryVisible { get; set; }
    [DefaultValue(true)]
    public bool IsFavoritesVisible { get; set; }
    [DefaultValue(true)]
    public bool IsSearchVisible { get; set; }
    [DefaultValue(true)]
    public bool NtpCoreVisibility { get; set; }

}