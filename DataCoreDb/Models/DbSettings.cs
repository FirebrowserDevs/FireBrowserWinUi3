using FireBrowserMultiCore;

namespace FireBrowserDataCore.Models
{

    public class DbSettings : FireBrowserMultiCore.Settings
    {

        public FireBrowserMultiCore.Settings Settings { get; set; }
        public DbSettings() : base(AuthService.CurrentUser?.UserSettings)
        {
            Settings = AuthService.CurrentUser?.UserSettings ?? new Settings();
        }

        public DbSettings(Settings settings) : base(settings)
        {
            Settings = settings;
        }

    }
}
