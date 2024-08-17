using FireBrowserWinUi3.Services.Contracts;
using FireBrowserWinUi3DataCore.Actions;
using FireBrowserWinUi3Exceptions;
using FireBrowserWinUi3MultiCore;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FireBrowserWinUi3.Services
{
    public class DatabaseServices : IDatabaseService
    {
        public async Task InsertUserSettings()
        {
            Batteries_V2.Init();

            if (!AuthService.IsUserAuthenticated)
                return;

            try
            {
                SettingsActions settingsActions = new SettingsActions(AuthService.CurrentUser.Username);
                string settingsDbPath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, AuthService.CurrentUser.Username, "Settings", "Settings.db");

                if (!File.Exists(settingsDbPath))
                {
                    await settingsActions.SettingsContext.Database.MigrateAsync();
                }

                if (File.Exists(settingsDbPath))
                {
                    if (await settingsActions.GetSettingsAsync() is null)
                    {
                        await settingsActions.InsertUserSettingsAsync(AppService.AppSettings);
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, "Error in Creating Settings Database");
            }
        }

        public async Task DatabaseCreationValidation()
        {
            if (!AuthService.IsUserAuthenticated)
                return;

            var tasks = new[]
            {
                ValidateDatabaseAsync<SettingsActions>("Settings.db"),
                ValidateDatabaseAsync<HistoryActions>("History.db"),
                ValidateDatabaseAsync<DownloadActions>("Downloads.db")
            };

            await Task.WhenAll(tasks);
        }

        private async Task ValidateDatabaseAsync<T>(string dbFileName) where T : class
        {
            try
            {
                string dbPath = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, AuthService.CurrentUser.Username, "Database", dbFileName);

                if (!File.Exists(dbPath))
                {
                    var actions = CreateActions<T>();
                    await actions.Database.MigrateAsync();
                }

                if (File.Exists(dbPath))
                {
                    var actions = CreateActions<T>();
                    await actions.Database.CanConnectAsync();
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, $"Error in Creating {dbFileName}");
            }
        }

        private dynamic CreateActions<T>() where T : class
        {
            if (typeof(T) == typeof(SettingsActions))
                return new SettingsActions(AuthService.CurrentUser.Username);

            if (typeof(T) == typeof(HistoryActions))
                return new HistoryActions(AuthService.CurrentUser?.Username);

            if (typeof(T) == typeof(DownloadActions))
                return new DownloadActions(AuthService.CurrentUser.Username);

            throw new InvalidOperationException("Unknown actions type");
        }

        private void HandleException(Exception ex, string message)
        {
            ExceptionLogger.LogException(ex);
            Console.WriteLine($"{message}: {ex.Message}");
        }
    }
}
