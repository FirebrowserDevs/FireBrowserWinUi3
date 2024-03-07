using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FireBrowserWinUi3DataCore.Migrations.Settings
{
    /// <inheritdoc />
    public partial class IntialSettingsSnapShotModel : Migration
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
                    Background = table.Column<int>(type: "INTEGER", nullable: false),
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
                    NtpCoreVisibility = table.Column<bool>(type: "INTEGER", nullable: false)
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
