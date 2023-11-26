using FireBrowserMultiCore;
using System.IO;
using System.Threading.Tasks;

namespace FireBrowserBusinessCore.Models
{
    public class Data
    {
        internal static string TotpFilePath { get; private set; }

        public static async Task Init()
        {

            string currentUsername = AuthService.CurrentUser.Username;

            string username = currentUsername;
            string userFolderPath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, username);
            string databaseFolderPath = Path.Combine(userFolderPath, "Database");


            string path = databaseFolderPath;

            TotpFilePath = Path.Combine(path, "2FA.json");
        }
    }
}
