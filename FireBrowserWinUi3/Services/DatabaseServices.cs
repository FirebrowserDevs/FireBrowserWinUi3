using FireBrowserWinUi3.Services.Contracts;
using FireBrowserWinUi3DataCore.Actions;
using FireBrowserWinUi3Exceptions;
using FireBrowserWinUi3MultiCore;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FireBrowserWinUi3.Services;
public class DatabaseServices : IDatabaseService
{
    public async Task<Task> InsertUserSettings()
    {
        Batteries_V2.Init();
        if (!AuthService.IsUserAuthenticated) return Task.FromResult(false); ;

        try
        {

            SettingsActions settingsActions = new SettingsActions(AuthService.CurrentUser.Username);
            if (!File.Exists(Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, AuthService.CurrentUser.Username, "Settings", "Settings.db")))
            {
                await settingsActions.SettingsContext.Database.MigrateAsync();
            }
            if (File.Exists(Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, AuthService.CurrentUser.Username, "Settings", "Settings.db")))
            {
                if (await settingsActions.GetSettingsAsync() is null)
                    await settingsActions.InsertUserSettingsAsync(FireBrowserWinUi3MultiCore.UserFolderManager.LoadUserSettings(FireBrowserWinUi3MultiCore.AuthService.CurrentUser));
            }

        }
        catch (Exception ex)
        {
            ExceptionLogger.LogException(ex);
            Console.WriteLine($"Error in Creating Settings Database: {ex.Message}");
            return Task.FromException(ex);
        }

        return Task.CompletedTask;
    }
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