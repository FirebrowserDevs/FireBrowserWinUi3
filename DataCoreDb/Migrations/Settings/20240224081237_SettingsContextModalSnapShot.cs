using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FireBrowserDataCore.Migrations.Settings
{
    /// <inheritdoc />
    public partial class SettingsContextModalSnapShot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    PackageName = table.Column<string>(type: "TEXT", nullable: false),
                    SelfPackageName = table.Column<string>(type: "TEXT", nullable: true),
                    DisableJavaScript = table.Column<string>(type: "TEXT", nullable: true),
                    DisablePassSave = table.Column<string>(type: "TEXT", nullable: true),
                    DisableWebMess = table.Column<string>(type: "TEXT", nullable: true),
                    DisableGenAutoFill = table.Column<string>(type: "TEXT", nullable: true),
                    ColorBackground = table.Column<string>(type: "TEXT", nullable: true),
                    StatusBar = table.Column<string>(type: "TEXT", nullable: true),
                    BrowserKeys = table.Column<string>(type: "TEXT", nullable: true),
                    BrowserScripts = table.Column<string>(type: "TEXT", nullable: true),
                    Useragent = table.Column<string>(type: "TEXT", nullable: true),
                    LightMode = table.Column<string>(type: "TEXT", nullable: true),
                    OpSw = table.Column<string>(type: "TEXT", nullable: true),
                    EngineFriendlyName = table.Column<string>(type: "TEXT", nullable: true),
                    SearchUrl = table.Column<string>(type: "TEXT", nullable: true),
                    ColorTool = table.Column<string>(type: "TEXT", nullable: true),
                    ColorTV = table.Column<string>(type: "TEXT", nullable: true),
                    Background = table.Column<string>(type: "TEXT", nullable: true),
                    Auto = table.Column<string>(type: "TEXT", nullable: true),
                    Lang = table.Column<string>(type: "TEXT", nullable: true),
                    ReadButton = table.Column<string>(type: "TEXT", nullable: true),
                    AdblockBtn = table.Column<string>(type: "TEXT", nullable: true),
                    Downloads = table.Column<string>(type: "TEXT", nullable: true),
                    Translate = table.Column<string>(type: "TEXT", nullable: true),
                    Favorites = table.Column<string>(type: "TEXT", nullable: true),
                    Historybtn = table.Column<string>(type: "TEXT", nullable: true),
                    QrCode = table.Column<string>(type: "TEXT", nullable: true),
                    FavoritesL = table.Column<string>(type: "TEXT", nullable: true),
                    ToolIcon = table.Column<string>(type: "TEXT", nullable: true),
                    DarkIcon = table.Column<string>(type: "TEXT", nullable: true),
                    OpenTabHandel = table.Column<string>(type: "TEXT", nullable: true),
                    NtpDateTime = table.Column<string>(type: "TEXT", nullable: true),
                    ExitDialog = table.Column<string>(type: "TEXT", nullable: true),
                    NtpTextColor = table.Column<string>(type: "TEXT", nullable: true),
                    ExceptionLog = table.Column<string>(type: "TEXT", nullable: true),
                    Eq2fa = table.Column<string>(type: "TEXT", nullable: true),
                    Eqfav = table.Column<string>(type: "TEXT", nullable: true),
                    EqHis = table.Column<string>(type: "TEXT", nullable: true),
                    Eqsets = table.Column<string>(type: "TEXT", nullable: true),
                    TrackPrevention = table.Column<string>(type: "TEXT", nullable: true),
                    ResourceSave = table.Column<string>(type: "TEXT", nullable: true),
                    ConfirmCloseDlg = table.Column<string>(type: "TEXT", nullable: true),
                    IsFavoritesToggled = table.Column<string>(type: "TEXT", nullable: true),
                    IsHistoryToggled = table.Column<string>(type: "TEXT", nullable: true),
                    isHistoryVisible = table.Column<string>(type: "TEXT", nullable: true),
                    isFavoritesVisible = table.Column<string>(type: "TEXT", nullable: true),
                    isSearchVisible = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.PackageName);
                    table.ForeignKey(
                        name: "FK_Settings_Settings_SelfPackageName",
                        column: x => x.SelfPackageName,
                        principalTable: "Settings",
                        principalColumn: "PackageName");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Settings_SelfPackageName",
                table: "Settings",
                column: "SelfPackageName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Settings");
        }
    }
}
