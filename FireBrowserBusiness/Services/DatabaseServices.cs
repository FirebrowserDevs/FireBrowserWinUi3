using FireBrowserDataCore.Actions;
using FireBrowserExceptions;
using FireBrowserMultiCore;
using FireBrowserWinUi3.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FireBrowserWinUi3.Services
{
    public class DatabaseServices : IDatabaseService
    {
        public async Task<Task> DatabaseCreationValidation()
        {
            if (!AuthService.IsUserAuthenticated) return Task.FromResult(false); ;

            try
            {

                SettingsActions settingsActions = new SettingsActions(AuthService.CurrentUser.Username);
                if (!File.Exists(Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, AuthService.CurrentUser.Username, "Settings", "Settings.db")))
                {
                    await settingsActions.SettingsContext.Database.MigrateAsync();
                }
                if (File.Exists(Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, AuthService.CurrentUser.Username, "Settings", "Settings.db")))
                    await settingsActions.SettingsContext.Database.CanConnectAsync();

            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                Console.WriteLine($"Error in Creating Settings Database: {ex.Message}");
            }


            try
            {

                HistoryActions historyActions = new HistoryActions(AuthService.CurrentUser?.Username);
                if (!File.Exists(Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, AuthService.CurrentUser.Username, "Database", "History.db")))
                {
                    await historyActions.HistoryContext.Database.MigrateAsync();
                }
                if (File.Exists(Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, AuthService.CurrentUser.Username, "Database", "History.db")))
                    await historyActions.HistoryContext.Database.CanConnectAsync();

            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                Console.WriteLine($"Error in Creating Settings Database: {ex.Message}");

            }

            try
            {

                DownloadActions settingsActions = new DownloadActions(AuthService.CurrentUser.Username);
                if (!File.Exists(Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, AuthService.CurrentUser.Username, "Database", "Downloads.db")))
                {
                    await settingsActions.DownloadContext.Database.MigrateAsync();
                }
                if (File.Exists(Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, AuthService.CurrentUser.Username, "Database", "Downloads.db")))
                    await settingsActions.DownloadContext.Database.CanConnectAsync();

            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                Console.WriteLine($"Error in Creating Settings Database: {ex.Message}");
            }

            return Task.CompletedTask;
        }
    }
}
