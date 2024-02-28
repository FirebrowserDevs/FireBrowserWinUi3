using FireBrowserMultiCore;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace FireBrowserDataCore
{
    public class SettingsContext : DbContext
    {
        public DbSet<Settings> Settings { get; set; }
        public string ConnectionPath { get; set; }
        public SettingsContext(string username)
        {
            ConnectionPath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, username, "Settings", "Settings.db");

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseSqlite($"Data Source={ConnectionPath}");

        }



    }
}
