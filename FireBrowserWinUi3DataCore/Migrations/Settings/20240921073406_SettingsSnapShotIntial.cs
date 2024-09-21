using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FireBrowserWinUi3DataCore.Migrations.Settings
{
    /// <inheritdoc />
    public partial class SettingsSnapShotIntial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    PackageName = table.Column<string>(type: "TEXT", nullable: false),
                    DisableJavaScript = table.Column<bool>(type: "INTEGER", nullable: false),
                    DisablePassSave = table.Column<bool>(type: "INTEGER", nullable: false),
                    DisableWebMess = table.Column<bool>(type: "INTEGER", nullable: false),
                    DisableGenAutoFill = table.Column<bool>(type: "INTEGER", nullable: false),
                    ColorBackground = table.Column<string>(type: "TEXT", nullable: true),
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

            //migrationBuilder.InsertData(
            //    table: "Settings",
            //    columns: new[] { "PackageName", "AdBlockerType", "AdblockBtn", "Auto", "BackButton", "Background", "BrowserKeys", "BrowserScripts", "ColorBackground", "ColorTV", "ColorTool", "ConfirmCloseDlg", "DarkIcon", "DisableGenAutoFill", "DisableJavaScript", "DisablePassSave", "DisableWebMess", "Downloads", "EngineFriendlyName", "Eq2fa", "EqHis", "Eqfav", "Eqsets", "ExceptionLog", "ExitDialog", "Favorites", "FavoritesL", "ForwardButton", "Historybtn", "HomeButton", "IsAdBlockerEnabled", "IsFavoritesToggled", "IsFavoritesVisible", "IsHistoryToggled", "IsHistoryVisible", "IsLogoVisible", "IsSearchBoxToggled", "IsSearchVisible", "IsTrendingVisible", "Lang", "LightMode", "NtpCoreVisibility", "NtpDateTime", "NtpTextColor", "OpSw", "OpenTabHandel", "PipMode", "QrCode", "ReadButton", "RefreshButton", "ResourceSave", "SearchUrl", "StatusBar", "ToolIcon", "TrackPrevention", "Translate", "Useragent" },
            //    values: new object[] { "FireBrowswerWinUi3_806e5648-8335-46f4-a406-56b28cbfe096", 0, true, false, true, 0, true, true, "#000000", "#000000", "#000000", true, true, false, false, false, false, true, "Google", true, false, false, false, "Low", false, true, true, true, true, true, false, false, true, false, true, true, false, true, true, "nl-NL", false, true, true, "#FFFFFF", true, false, false, true, true, true, false, "https://www.google.com/search?q=", true, true, 2, true, "WebView" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Settings");
        }
    }
}
