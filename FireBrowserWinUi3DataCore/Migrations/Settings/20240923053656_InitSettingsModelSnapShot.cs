using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FireBrowserWinUi3DataCore.Migrations.Settings
{
    /// <inheritdoc />
    public partial class InitSettingsModelSnapShot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "CREATE TABLE IF NOT EXISTS \"__EFMigrationsHistory\" (\r\n    \"MigrationId\" TEXT NOT NULL CONSTRAINT \"PK___EFMigrationsHistory\" PRIMARY KEY,\r\n    \"ProductVersion\" TEXT NOT NULL\r\n);\r\n\r\nBEGIN TRANSACTION;\r\n\r\nCREATE TABLE \"IntialSettings\" (\r\n    \"PackageName\" TEXT NOT NULL CONSTRAINT \"PK_InitSettings\" PRIMARY KEY,\r\n    \"Gender\" TEXT NOT NULL, \r\n    \"DisableJavaScript\" INTEGER NOT NULL,\r\n    \"DisablePassSave\" INTEGER NOT NULL,\r\n    \"DisableWebMess\" INTEGER NOT NULL,\r\n    \"DisableGenAutoFill\" INTEGER NOT NULL,\r\n    \"ColorBackground\" TEXT NULL,\r\n    \"Gender\" TEXT NULL,\r\n    \"StatusBar\" INTEGER NOT NULL,\r\n    \"BrowserKeys\" INTEGER NOT NULL,\r\n    \"BrowserScripts\" INTEGER NOT NULL,\r\n    \"Useragent\" TEXT NULL,\r\n    \"LightMode\" INTEGER NOT NULL,\r\n    \"OpSw\" INTEGER NOT NULL,\r\n    \"EngineFriendlyName\" TEXT NULL,\r\n    \"SearchUrl\" TEXT NULL,\r\n    \"ColorTool\" TEXT NULL,\r\n    \"ColorTV\" TEXT NULL,\r\n    \"AdBlockerType\" INTEGER NOT NULL,\r\n    \"Background\" INTEGER NOT NULL,\r\n    \"IsAdBlockerEnabled\" INTEGER NOT NULL,\r\n    \"Auto\" INTEGER NOT NULL,\r\n    \"Lang\" TEXT NULL,\r\n    \"ReadButton\" INTEGER NOT NULL,\r\n    \"AdblockBtn\" INTEGER NOT NULL,\r\n    \"Downloads\" INTEGER NOT NULL,\r\n    \"Translate\" INTEGER NOT NULL,\r\n    \"Favorites\" INTEGER NOT NULL,\r\n    \"Historybtn\" INTEGER NOT NULL,\r\n    \"QrCode\" INTEGER NOT NULL,\r\n    \"FavoritesL\" INTEGER NOT NULL,\r\n    \"ToolIcon\" INTEGER NOT NULL,\r\n    \"DarkIcon\" INTEGER NOT NULL,\r\n    \"OpenTabHandel\" INTEGER NOT NULL,\r\n    \"BackButton\" INTEGER NOT NULL,\r\n    \"ForwardButton\" INTEGER NOT NULL,\r\n    \"RefreshButton\" INTEGER NOT NULL,\r\n    \"IsLogoVisible\" INTEGER NOT NULL,\r\n    \"HomeButton\" INTEGER NOT NULL,\r\n    \"PipMode\" INTEGER NOT NULL,\r\n    \"NtpDateTime\" INTEGER NOT NULL,\r\n    \"ExitDialog\" INTEGER NOT NULL,\r\n    \"NtpTextColor\" TEXT NULL,\r\n    \"ExceptionLog\" TEXT NULL,\r\n    \"Eq2fa\" INTEGER NOT NULL,\r\n    \"Eqfav\" INTEGER NOT NULL,\r\n    \"EqHis\" INTEGER NOT NULL,\r\n    \"Eqsets\" INTEGER NOT NULL,\r\n    \"TrackPrevention\" INTEGER NOT NULL,\r\n    \"ResourceSave\" INTEGER NOT NULL,\r\n    \"ConfirmCloseDlg\" INTEGER NOT NULL,\r\n    \"IsFavoritesToggled\" INTEGER NOT NULL,\r\n    \"IsSearchBoxToggled\" INTEGER NOT NULL,\r\n    \"IsHistoryToggled\" INTEGER NOT NULL,\r\n    \"IsHistoryVisible\" INTEGER NOT NULL,\r\n    \"IsFavoritesVisible\" INTEGER NOT NULL,\r\n    \"IsSearchVisible\" INTEGER NOT NULL,\r\n    \"IsTrendingVisible\" INTEGER NOT NULL,\r\n    \"NtpCoreVisibility\" INTEGER NOT NULL\r\n);\r\n\r\nINSERT INTO \"__EFMigrationsHistory\" (\"MigrationId\", \"ProductVersion\")\r\nVALUES ('20240923183422_InitialSettingsSnapShot', '9.0.0-preview.7.24405.3');\r\n\r\nCOMMIT;\r\n\r\n");

            migrationBuilder.CreateTable(
                name: "Settings",
                schema: "CREATE TABLE IF NOT EXISTS \"__EFMigrationsHistory\" (\r\n    \"MigrationId\" TEXT NOT NULL CONSTRAINT \"PK___EFMigrationsHistory\" PRIMARY KEY,\r\n    \"ProductVersion\" TEXT NOT NULL\r\n);\r\n\r\nBEGIN TRANSACTION;\r\n\r\nCREATE TABLE \"IntialSettings\" (\r\n    \"PackageName\" TEXT NOT NULL CONSTRAINT \"PK_InitSettings\" PRIMARY KEY,\r\n    \"Gender\" TEXT NOT NULL, \r\n    \"DisableJavaScript\" INTEGER NOT NULL,\r\n    \"DisablePassSave\" INTEGER NOT NULL,\r\n    \"DisableWebMess\" INTEGER NOT NULL,\r\n    \"DisableGenAutoFill\" INTEGER NOT NULL,\r\n    \"ColorBackground\" TEXT NULL,\r\n    \"Gender\" TEXT NULL,\r\n    \"StatusBar\" INTEGER NOT NULL,\r\n    \"BrowserKeys\" INTEGER NOT NULL,\r\n    \"BrowserScripts\" INTEGER NOT NULL,\r\n    \"Useragent\" TEXT NULL,\r\n    \"LightMode\" INTEGER NOT NULL,\r\n    \"OpSw\" INTEGER NOT NULL,\r\n    \"EngineFriendlyName\" TEXT NULL,\r\n    \"SearchUrl\" TEXT NULL,\r\n    \"ColorTool\" TEXT NULL,\r\n    \"ColorTV\" TEXT NULL,\r\n    \"AdBlockerType\" INTEGER NOT NULL,\r\n    \"Background\" INTEGER NOT NULL,\r\n    \"IsAdBlockerEnabled\" INTEGER NOT NULL,\r\n    \"Auto\" INTEGER NOT NULL,\r\n    \"Lang\" TEXT NULL,\r\n    \"ReadButton\" INTEGER NOT NULL,\r\n    \"AdblockBtn\" INTEGER NOT NULL,\r\n    \"Downloads\" INTEGER NOT NULL,\r\n    \"Translate\" INTEGER NOT NULL,\r\n    \"Favorites\" INTEGER NOT NULL,\r\n    \"Historybtn\" INTEGER NOT NULL,\r\n    \"QrCode\" INTEGER NOT NULL,\r\n    \"FavoritesL\" INTEGER NOT NULL,\r\n    \"ToolIcon\" INTEGER NOT NULL,\r\n    \"DarkIcon\" INTEGER NOT NULL,\r\n    \"OpenTabHandel\" INTEGER NOT NULL,\r\n    \"BackButton\" INTEGER NOT NULL,\r\n    \"ForwardButton\" INTEGER NOT NULL,\r\n    \"RefreshButton\" INTEGER NOT NULL,\r\n    \"IsLogoVisible\" INTEGER NOT NULL,\r\n    \"HomeButton\" INTEGER NOT NULL,\r\n    \"PipMode\" INTEGER NOT NULL,\r\n    \"NtpDateTime\" INTEGER NOT NULL,\r\n    \"ExitDialog\" INTEGER NOT NULL,\r\n    \"NtpTextColor\" TEXT NULL,\r\n    \"ExceptionLog\" TEXT NULL,\r\n    \"Eq2fa\" INTEGER NOT NULL,\r\n    \"Eqfav\" INTEGER NOT NULL,\r\n    \"EqHis\" INTEGER NOT NULL,\r\n    \"Eqsets\" INTEGER NOT NULL,\r\n    \"TrackPrevention\" INTEGER NOT NULL,\r\n    \"ResourceSave\" INTEGER NOT NULL,\r\n    \"ConfirmCloseDlg\" INTEGER NOT NULL,\r\n    \"IsFavoritesToggled\" INTEGER NOT NULL,\r\n    \"IsSearchBoxToggled\" INTEGER NOT NULL,\r\n    \"IsHistoryToggled\" INTEGER NOT NULL,\r\n    \"IsHistoryVisible\" INTEGER NOT NULL,\r\n    \"IsFavoritesVisible\" INTEGER NOT NULL,\r\n    \"IsSearchVisible\" INTEGER NOT NULL,\r\n    \"IsTrendingVisible\" INTEGER NOT NULL,\r\n    \"NtpCoreVisibility\" INTEGER NOT NULL\r\n);\r\n\r\nINSERT INTO \"__EFMigrationsHistory\" (\"MigrationId\", \"ProductVersion\")\r\nVALUES ('20240923183422_InitialSettingsSnapShot', '9.0.0-preview.7.24405.3');\r\n\r\nCOMMIT;\r\n\r\n",
                columns: table => new
                {
                    PackageName = table.Column<string>(type: "TEXT", nullable: false),
                    DisableJavaScript = table.Column<bool>(type: "INTEGER", nullable: false),
                    DisablePassSave = table.Column<bool>(type: "INTEGER", nullable: false),
                    DisableWebMess = table.Column<bool>(type: "INTEGER", nullable: false),
                    DisableGenAutoFill = table.Column<bool>(type: "INTEGER", nullable: false),
                    ColorBackground = table.Column<string>(type: "TEXT", nullable: true),
                    Gender = table.Column<string>(type: "TEXT", nullable: true),
                    StatusBar = table.Column<bool>(type: "INTEGER", nullable: false),
                    BrowserKeys = table.Column<bool>(type: "INTEGER", nullable: false),
                    BrowserScripts = table.Column<bool>(type: "INTEGER", nullable: false),
                    Useragent = table.Column<string>(type: "TEXT", nullable: true),
                    LightMode = table.Column<bool>(type: "INTEGER", nullable: false),
                    OpSw = table.Column<bool>(type: "INTEGER", nullable: false),
                    EngineFriendlyName = table.Column<string>(type: "TEXT", nullable: true),
                    SearchUrl = table.Column<string>(type: "TEXT", nullable: true),
                    ColorTool = table.Column<string>(type: "TEXT", nullable: true),
                    ColorTV = table.Column<string>(type: "TEXT", nullable: true),
                    AdBlockerType = table.Column<int>(type: "INTEGER", nullable: false),
                    Background = table.Column<int>(type: "INTEGER", nullable: false),
                    IsAdBlockerEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    Auto = table.Column<bool>(type: "INTEGER", nullable: false),
                    Lang = table.Column<string>(type: "TEXT", nullable: true),
                    ReadButton = table.Column<bool>(type: "INTEGER", nullable: false),
                    AdblockBtn = table.Column<bool>(type: "INTEGER", nullable: false),
                    Downloads = table.Column<bool>(type: "INTEGER", nullable: false),
                    Translate = table.Column<bool>(type: "INTEGER", nullable: false),
                    Favorites = table.Column<bool>(type: "INTEGER", nullable: false),
                    Historybtn = table.Column<bool>(type: "INTEGER", nullable: false),
                    QrCode = table.Column<bool>(type: "INTEGER", nullable: false),
                    FavoritesL = table.Column<bool>(type: "INTEGER", nullable: false),
                    ToolIcon = table.Column<bool>(type: "INTEGER", nullable: false),
                    DarkIcon = table.Column<bool>(type: "INTEGER", nullable: false),
                    OpenTabHandel = table.Column<bool>(type: "INTEGER", nullable: false),
                    BackButton = table.Column<bool>(type: "INTEGER", nullable: false),
                    ForwardButton = table.Column<bool>(type: "INTEGER", nullable: false),
                    RefreshButton = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsLogoVisible = table.Column<bool>(type: "INTEGER", nullable: false),
                    HomeButton = table.Column<bool>(type: "INTEGER", nullable: false),
                    PipMode = table.Column<bool>(type: "INTEGER", nullable: false),
                    NtpDateTime = table.Column<bool>(type: "INTEGER", nullable: false),
                    ExitDialog = table.Column<bool>(type: "INTEGER", nullable: false),
                    NtpTextColor = table.Column<string>(type: "TEXT", nullable: true),
                    ExceptionLog = table.Column<string>(type: "TEXT", nullable: true),
                    Eq2fa = table.Column<bool>(type: "INTEGER", nullable: false),
                    Eqfav = table.Column<bool>(type: "INTEGER", nullable: false),
                    EqHis = table.Column<bool>(type: "INTEGER", nullable: false),
                    Eqsets = table.Column<bool>(type: "INTEGER", nullable: false),
                    TrackPrevention = table.Column<int>(type: "INTEGER", nullable: false),
                    ResourceSave = table.Column<bool>(type: "INTEGER", nullable: false),
                    ConfirmCloseDlg = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsFavoritesToggled = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsSearchBoxToggled = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsHistoryToggled = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsHistoryVisible = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsFavoritesVisible = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsSearchVisible = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsTrendingVisible = table.Column<bool>(type: "INTEGER", nullable: false),
                    NtpCoreVisibility = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.PackageName);
                });

            migrationBuilder.InsertData(
                schema: "CREATE TABLE IF NOT EXISTS \"__EFMigrationsHistory\" (\r\n    \"MigrationId\" TEXT NOT NULL CONSTRAINT \"PK___EFMigrationsHistory\" PRIMARY KEY,\r\n    \"ProductVersion\" TEXT NOT NULL\r\n);\r\n\r\nBEGIN TRANSACTION;\r\n\r\nCREATE TABLE \"IntialSettings\" (\r\n    \"PackageName\" TEXT NOT NULL CONSTRAINT \"PK_InitSettings\" PRIMARY KEY,\r\n    \"Gender\" TEXT NOT NULL, \r\n    \"DisableJavaScript\" INTEGER NOT NULL,\r\n    \"DisablePassSave\" INTEGER NOT NULL,\r\n    \"DisableWebMess\" INTEGER NOT NULL,\r\n    \"DisableGenAutoFill\" INTEGER NOT NULL,\r\n    \"ColorBackground\" TEXT NULL,\r\n    \"Gender\" TEXT NULL,\r\n    \"StatusBar\" INTEGER NOT NULL,\r\n    \"BrowserKeys\" INTEGER NOT NULL,\r\n    \"BrowserScripts\" INTEGER NOT NULL,\r\n    \"Useragent\" TEXT NULL,\r\n    \"LightMode\" INTEGER NOT NULL,\r\n    \"OpSw\" INTEGER NOT NULL,\r\n    \"EngineFriendlyName\" TEXT NULL,\r\n    \"SearchUrl\" TEXT NULL,\r\n    \"ColorTool\" TEXT NULL,\r\n    \"ColorTV\" TEXT NULL,\r\n    \"AdBlockerType\" INTEGER NOT NULL,\r\n    \"Background\" INTEGER NOT NULL,\r\n    \"IsAdBlockerEnabled\" INTEGER NOT NULL,\r\n    \"Auto\" INTEGER NOT NULL,\r\n    \"Lang\" TEXT NULL,\r\n    \"ReadButton\" INTEGER NOT NULL,\r\n    \"AdblockBtn\" INTEGER NOT NULL,\r\n    \"Downloads\" INTEGER NOT NULL,\r\n    \"Translate\" INTEGER NOT NULL,\r\n    \"Favorites\" INTEGER NOT NULL,\r\n    \"Historybtn\" INTEGER NOT NULL,\r\n    \"QrCode\" INTEGER NOT NULL,\r\n    \"FavoritesL\" INTEGER NOT NULL,\r\n    \"ToolIcon\" INTEGER NOT NULL,\r\n    \"DarkIcon\" INTEGER NOT NULL,\r\n    \"OpenTabHandel\" INTEGER NOT NULL,\r\n    \"BackButton\" INTEGER NOT NULL,\r\n    \"ForwardButton\" INTEGER NOT NULL,\r\n    \"RefreshButton\" INTEGER NOT NULL,\r\n    \"IsLogoVisible\" INTEGER NOT NULL,\r\n    \"HomeButton\" INTEGER NOT NULL,\r\n    \"PipMode\" INTEGER NOT NULL,\r\n    \"NtpDateTime\" INTEGER NOT NULL,\r\n    \"ExitDialog\" INTEGER NOT NULL,\r\n    \"NtpTextColor\" TEXT NULL,\r\n    \"ExceptionLog\" TEXT NULL,\r\n    \"Eq2fa\" INTEGER NOT NULL,\r\n    \"Eqfav\" INTEGER NOT NULL,\r\n    \"EqHis\" INTEGER NOT NULL,\r\n    \"Eqsets\" INTEGER NOT NULL,\r\n    \"TrackPrevention\" INTEGER NOT NULL,\r\n    \"ResourceSave\" INTEGER NOT NULL,\r\n    \"ConfirmCloseDlg\" INTEGER NOT NULL,\r\n    \"IsFavoritesToggled\" INTEGER NOT NULL,\r\n    \"IsSearchBoxToggled\" INTEGER NOT NULL,\r\n    \"IsHistoryToggled\" INTEGER NOT NULL,\r\n    \"IsHistoryVisible\" INTEGER NOT NULL,\r\n    \"IsFavoritesVisible\" INTEGER NOT NULL,\r\n    \"IsSearchVisible\" INTEGER NOT NULL,\r\n    \"IsTrendingVisible\" INTEGER NOT NULL,\r\n    \"NtpCoreVisibility\" INTEGER NOT NULL\r\n);\r\n\r\nINSERT INTO \"__EFMigrationsHistory\" (\"MigrationId\", \"ProductVersion\")\r\nVALUES ('20240923183422_InitialSettingsSnapShot', '9.0.0-preview.7.24405.3');\r\n\r\nCOMMIT;\r\n\r\n",
                table: "Settings",
                columns: new[] { "PackageName", "AdBlockerType", "AdblockBtn", "Auto", "BackButton", "Background", "BrowserKeys", "BrowserScripts", "ColorBackground", "ColorTV", "ColorTool", "ConfirmCloseDlg", "DarkIcon", "DisableGenAutoFill", "DisableJavaScript", "DisablePassSave", "DisableWebMess", "Downloads", "EngineFriendlyName", "Eq2fa", "EqHis", "Eqfav", "Eqsets", "ExceptionLog", "ExitDialog", "Favorites", "FavoritesL", "ForwardButton", "Gender", "Historybtn", "HomeButton", "IsAdBlockerEnabled", "IsFavoritesToggled", "IsFavoritesVisible", "IsHistoryToggled", "IsHistoryVisible", "IsLogoVisible", "IsSearchBoxToggled", "IsSearchVisible", "IsTrendingVisible", "Lang", "LightMode", "NtpCoreVisibility", "NtpDateTime", "NtpTextColor", "OpSw", "OpenTabHandel", "PipMode", "QrCode", "ReadButton", "RefreshButton", "ResourceSave", "SearchUrl", "StatusBar", "ToolIcon", "TrackPrevention", "Translate", "Useragent" },
                values: new object[] { "FireBrowswerWinUi3_", 0, true, false, true, 0, true, true, "#000000", "#000000", "#000000", true, true, false, false, false, false, true, "Google", true, false, false, false, "Low", false, true, true, true, "Male", true, true, false, false, true, false, true, true, false, true, true, "nl-NL", false, true, true, "#FFFFFF", true, false, false, true, true, true, false, "https://www.google.com/search?q=", true, true, 2, true, "WebView" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Settings",
                schema: "CREATE TABLE IF NOT EXISTS \"__EFMigrationsHistory\" (\r\n    \"MigrationId\" TEXT NOT NULL CONSTRAINT \"PK___EFMigrationsHistory\" PRIMARY KEY,\r\n    \"ProductVersion\" TEXT NOT NULL\r\n);\r\n\r\nBEGIN TRANSACTION;\r\n\r\nCREATE TABLE \"IntialSettings\" (\r\n    \"PackageName\" TEXT NOT NULL CONSTRAINT \"PK_InitSettings\" PRIMARY KEY,\r\n    \"Gender\" TEXT NOT NULL, \r\n    \"DisableJavaScript\" INTEGER NOT NULL,\r\n    \"DisablePassSave\" INTEGER NOT NULL,\r\n    \"DisableWebMess\" INTEGER NOT NULL,\r\n    \"DisableGenAutoFill\" INTEGER NOT NULL,\r\n    \"ColorBackground\" TEXT NULL,\r\n    \"Gender\" TEXT NULL,\r\n    \"StatusBar\" INTEGER NOT NULL,\r\n    \"BrowserKeys\" INTEGER NOT NULL,\r\n    \"BrowserScripts\" INTEGER NOT NULL,\r\n    \"Useragent\" TEXT NULL,\r\n    \"LightMode\" INTEGER NOT NULL,\r\n    \"OpSw\" INTEGER NOT NULL,\r\n    \"EngineFriendlyName\" TEXT NULL,\r\n    \"SearchUrl\" TEXT NULL,\r\n    \"ColorTool\" TEXT NULL,\r\n    \"ColorTV\" TEXT NULL,\r\n    \"AdBlockerType\" INTEGER NOT NULL,\r\n    \"Background\" INTEGER NOT NULL,\r\n    \"IsAdBlockerEnabled\" INTEGER NOT NULL,\r\n    \"Auto\" INTEGER NOT NULL,\r\n    \"Lang\" TEXT NULL,\r\n    \"ReadButton\" INTEGER NOT NULL,\r\n    \"AdblockBtn\" INTEGER NOT NULL,\r\n    \"Downloads\" INTEGER NOT NULL,\r\n    \"Translate\" INTEGER NOT NULL,\r\n    \"Favorites\" INTEGER NOT NULL,\r\n    \"Historybtn\" INTEGER NOT NULL,\r\n    \"QrCode\" INTEGER NOT NULL,\r\n    \"FavoritesL\" INTEGER NOT NULL,\r\n    \"ToolIcon\" INTEGER NOT NULL,\r\n    \"DarkIcon\" INTEGER NOT NULL,\r\n    \"OpenTabHandel\" INTEGER NOT NULL,\r\n    \"BackButton\" INTEGER NOT NULL,\r\n    \"ForwardButton\" INTEGER NOT NULL,\r\n    \"RefreshButton\" INTEGER NOT NULL,\r\n    \"IsLogoVisible\" INTEGER NOT NULL,\r\n    \"HomeButton\" INTEGER NOT NULL,\r\n    \"PipMode\" INTEGER NOT NULL,\r\n    \"NtpDateTime\" INTEGER NOT NULL,\r\n    \"ExitDialog\" INTEGER NOT NULL,\r\n    \"NtpTextColor\" TEXT NULL,\r\n    \"ExceptionLog\" TEXT NULL,\r\n    \"Eq2fa\" INTEGER NOT NULL,\r\n    \"Eqfav\" INTEGER NOT NULL,\r\n    \"EqHis\" INTEGER NOT NULL,\r\n    \"Eqsets\" INTEGER NOT NULL,\r\n    \"TrackPrevention\" INTEGER NOT NULL,\r\n    \"ResourceSave\" INTEGER NOT NULL,\r\n    \"ConfirmCloseDlg\" INTEGER NOT NULL,\r\n    \"IsFavoritesToggled\" INTEGER NOT NULL,\r\n    \"IsSearchBoxToggled\" INTEGER NOT NULL,\r\n    \"IsHistoryToggled\" INTEGER NOT NULL,\r\n    \"IsHistoryVisible\" INTEGER NOT NULL,\r\n    \"IsFavoritesVisible\" INTEGER NOT NULL,\r\n    \"IsSearchVisible\" INTEGER NOT NULL,\r\n    \"IsTrendingVisible\" INTEGER NOT NULL,\r\n    \"NtpCoreVisibility\" INTEGER NOT NULL\r\n);\r\n\r\nINSERT INTO \"__EFMigrationsHistory\" (\"MigrationId\", \"ProductVersion\")\r\nVALUES ('20240923183422_InitialSettingsSnapShot', '9.0.0-preview.7.24405.3');\r\n\r\nCOMMIT;\r\n\r\n");
        }
    }
}
