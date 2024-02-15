// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

using FireBrowserDataCore.Models;
using FireBrowserMultiCore;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace FireBrowserDataCore
{
    public class HistoryContext : DbContext
    {
        public DbSet<HistoryItem> Urls { get; set; }
        //public DbSet<DownloadItem> Downloads { get; set; }
        //public DbSet<DbUser> Users { get; set; }

        public string ConnectionPath { get; set; }
        public HistoryContext(string username)
        {
            ConnectionPath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, username, "Database", "History.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlite($"Data Source={ConnectionPath}");

    }
}
