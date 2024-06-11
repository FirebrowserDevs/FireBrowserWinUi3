using System.IO;

namespace FireBrowserWinUi3MultiCore;
public static class UserFolderManager
{
    private static readonly string SettingsFolderName = "Settings";
    private static readonly string DatabaseFolderName = "Database";
    private static readonly string[] SubFolderNames = { SettingsFolderName, DatabaseFolderName, "Browser", "Modules" };

    public static void CreateUserFolders(User user)
    {
        string userFolderPath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, user.Username);

        foreach (var folderName in SubFolderNames)
        {
            Directory.CreateDirectory(Path.Combine(userFolderPath, folderName));
        }     
    }
}