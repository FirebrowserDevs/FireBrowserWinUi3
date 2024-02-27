using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FireBrowserMultiCore;
public class Settings
{

    void DefaultSettings()
    {
        var self = this;
        self.PackageName = "FireBrowswerWinUi3";
        self.DisableJavaScript = "0";
        self.DisablePassSave = "0";
        self.DisableWebMess = "0";
        self.DisableGenAutoFill = "0";
        self.ColorBackground = "#000000";
        self.StatusBar = "1";
        self.BrowserKeys = "1";
        self.BrowserScripts = "1";
        self.Useragent = "WebView";
        self.LightMode = "0";
        self.OpSw = "1";
        self.EngineFriendlyName = "Google";
        self.SearchUrl = "https://www.google.com/search?q=";
        self.ColorTool = "#000000";
        self.ColorTV = "#000000";
        self.Background = "1";
        self.Auto = "0";
        self.Lang = "nl-NL";
        self.ReadButton = "1";
        self.AdblockBtn = "1";
        self.Downloads = "1";
        self.Translate = "1";
        self.Favorites = "1";
        self.Historybtn = "1";
        self.QrCode = "1";
        self.FavoritesL = "1";
        self.ToolIcon = "1";
        self.DarkIcon = "1";
        self.OpenTabHandel = "0";
        self.NtpDateTime = "0";
        self.ExitDialog = "0";
        self.NtpTextColor = "#000000";
        self.ExceptionLog = "Low";
        self.Eq2fa = "1";
        self.Eqfav = "0";
        self.EqHis = "0";
        self.Eqsets = "0";
        self.TrackPrevention = "2";
        self.ResourceSave = "0";
        self.ConfirmCloseDlg = "1";
        self.IsHistoryToggled = "0";
        self.IsFavoritesToggled = "0";
        self.IsFavoritesVisible = "1";
        self.IsHistoryVisible = "1";
        self.IsSearchVisible = "1";
        self.NtpCoreVisibility = "1";

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

    [DefaultValue("0")]
    public string DisableJavaScript { get; set; } // Use "0" for false, "1" for true
    [DefaultValue("0")]
    public string DisablePassSave { get; set; } // Use "0" for false, "1" for true
    [DefaultValue("0")]
    public string DisableWebMess { get; set; } // Use "0" for false, "1" for true
    [DefaultValue("0")]
    public string DisableGenAutoFill { get; set; } // Use "0" for false, "1" for true
    [DefaultValue("#000000")]
    public string ColorBackground { get; set; }
    [DefaultValue("1")]
    public string StatusBar { get; set; } // Use "0" for false, "1" for true
    [DefaultValue("1")]
    public string BrowserKeys { get; set; } // Use "0" for false, "1" for true
    [DefaultValue("1")]
    public string BrowserScripts { get; set; } // Use "0" for false, "1" for true
    [DefaultValue("WebView")]
    public string Useragent { get; set; }
    [DefaultValue("0")]
    public string LightMode { get; set; } // Use "0" for false, "1" for true
    [DefaultValue("1")]
    public string OpSw { get; set; } // Use "0" for false, "1" for true
    [DefaultValue("Google")]
    public string EngineFriendlyName { get; set; }
    [DefaultValue("https://www.google.com/search?q=")]
    public string SearchUrl { get; set; }
    [DefaultValue("#000000")]
    public string ColorTool { get; set; }
    [DefaultValue("#000000")]
    public string ColorTV { get; set; }
    [DefaultValue("1")]
    public string Background { get; set; } // Use "0" for false, "1" for true
    [DefaultValue("0")]
    public string Auto { get; set; } // Use "0" for false, "1" for true
    [DefaultValue("nl-NL")]
    public string Lang { get; set; }
    [DefaultValue("1")]
    public string ReadButton { get; set; }
    [DefaultValue("1")]
    public string AdblockBtn { get; set; }
    [DefaultValue("1")]
    public string Downloads { get; set; }
    [DefaultValue("1")]
    public string Translate { get; set; }
    [DefaultValue("1")]
    public string Favorites { get; set; }
    [DefaultValue("1")]
    public string Historybtn { get; set; }
    [DefaultValue("1")]
    public string QrCode { get; set; }
    [DefaultValue("1")]
    public string FavoritesL { get; set; }
    [DefaultValue("1")]
    public string ToolIcon { get; set; }

    [DefaultValue("1")]
    public string DarkIcon { get; set; }
    [DefaultValue("0")]
    public string OpenTabHandel { get; set; }
    [DefaultValue("0")]
    public string NtpDateTime { get; set; }

    [DefaultValue("0")]
    public string ExitDialog { get; set; }
    [DefaultValue("#000000")]
    public string NtpTextColor { get; set; }
    [DefaultValue("Low")]
    public string ExceptionLog { get; set; }
    [DefaultValue("1")]
    public string Eq2fa { get; set; }
    [DefaultValue("0")]
    public string Eqfav { get; set; }
    [DefaultValue("0")]
    public string EqHis { get; set; }
    [DefaultValue("0")]
    public string Eqsets { get; set; }
    [DefaultValue("2")]

    public string TrackPrevention { get; set; }
    [DefaultValue("0")]
    public string ResourceSave { get; set; }
    [DefaultValue("1")]
    public string ConfirmCloseDlg { get; set; }
    [DefaultValue("0")]
    public string IsFavoritesToggled { get; set; }
    [DefaultValue("0")]
    public string IsHistoryToggled { get; set; }
    [DefaultValue("1")]
    public string IsHistoryVisible { get; set; }
    [DefaultValue("1")]
    public string IsFavoritesVisible { get; set; }
    [DefaultValue("1")]
    public string IsSearchVisible { get; set; }
    [DefaultValue("1")]
    public string NtpCoreVisibility { get; set; }

}