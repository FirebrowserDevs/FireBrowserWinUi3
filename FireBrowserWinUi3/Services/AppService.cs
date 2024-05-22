using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using FireBrowserWinUi3.Services.Messages;
using FireBrowserWinUi3Core.Helpers;
using FireBrowserWinUi3MultiCore;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using WinRT.Interop;
using Microsoft.UI.Windowing;
using FireBrowserWinUi3Exceptions;
using System.Text.Json;
using Windows.ApplicationModel;
using System.Windows.Forms;
using FireBrowserWinUi3.Setup;
using Windows.Graphics;
using FireBrowserWinUi3DataCore.Actions;
using Microsoft.EntityFrameworkCore;
using FireBrowserWinUi3.Controls;

namespace FireBrowserWinUi3.Services
{
    static public class AppService
    {
        /*static Tuple<Window, Window> AP_WINDOWS { get; set; }
        // Instance.AP_WINDOWS = new Tuple<Window, Window>(new MainWindow(), new SetupWindow());*/

        static public Window ActiveWindow { get; set; }
        static public Settings AppSettings { get; set; }

        static public CancellationToken CancellationToken { get; set; }


        static public async Task<Task> WindowsController(CancellationToken cancellationToken)
        {

            try
            {
                string changeUsernameFilePath = Path.Combine(Path.GetTempPath(), "changeusername.json");
                string patchFilePath = Path.Combine(Path.GetTempPath(), "Patch.ptc");


                if (!Directory.Exists(UserDataManager.CoreFolderPath))
                {
                    // load settings for first time to default and run..
                    AppSettings = new Settings(true).Self;
                    ActiveWindow = new SetupWindow();

                    ActiveWindow.Closed += (s, e) =>
                    {
                        WindowsController(cancellationToken).ConfigureAwait(false);
                    };

                    ActiveWindow.Activate();

                    return Task.CompletedTask;
                }
                // change name Func<NewUserChanged> roll 
                if (File.Exists(changeUsernameFilePath))
                {

                    ActiveWindow = new ChangeUsernameCore();

                    ActiveWindow.Closed += (s, e) =>
                    {
                        AuthService.IsUserNameChanging = false;
                        WindowsController(cancellationToken).ConfigureAwait(false);
                    };

                    ActiveWindow.Activate();

                    return Task.CompletedTask;

                }
                if (File.Exists(patchFilePath))
                {

                    ActiveWindow = new Patcher();

                    ActiveWindow.Closed += (s, e) =>
                    {
                        AuthService.IsUserNameChanging = false;
                        WindowsController(cancellationToken).ConfigureAwait(false);
                    };

                    ActiveWindow.Activate();

                    return Task.CompletedTask;

                }
                // no user and not for change name hence currentuser is set from above changeuserName
                if (AuthService.CurrentUser is null)
                {
                    var evt = AppInstance.GetActivatedEventArgs();
                    ProtocolActivatedEventArgs protocolArgs = evt as ProtocolActivatedEventArgs;

                    if (protocolArgs != null && protocolArgs.Kind == ActivationKind.Protocol)
                    {
                        string url = protocolArgs.Uri.ToString();

                        if (url.StartsWith("http") || url.StartsWith("https"))
                        {
                            AppArguments.UrlArgument = url; // Standard web URL
                            CheckNormal();
                        }
                        else if (url.StartsWith("firebrowserwinui://"))
                        {
                            AppArguments.FireBrowserArgument = url;
                            CheckNormal();
                        }
                        else if (url.StartsWith("firebrowseruser://"))
                        {
                            AppArguments.FireUser = url;

                            // Extract the username after 'firebrowseruser://'
                            string usernameSegment = url.Replace("firebrowseruser://", ""); // Remove the prefix
                            string[] urlParts = usernameSegment.Split('/', StringSplitOptions.RemoveEmptyEntries);
                            string username = urlParts.FirstOrDefault(); // Retrieve the first segment as the username

                            // Authenticate the extracted username using your authentication service
                            if (!string.IsNullOrEmpty(username))
                            {
                                CheckNormal(username);
                            }

                            // No need to activate the window here
                        }
                        else if (url.StartsWith("firebrowserincog://"))
                        {
                            AppArguments.FireBrowserIncog = url;
                            CheckNormal(); // Custom protocol for FireBrowser
                        }
                        else if (url.Contains(".pdf"))
                        {
                            AppArguments.FireBrowserPdf = url;
                            CheckNormal();
                        }
                    }



                    ActiveWindow = new UserCentral();

                    ActiveWindow.Closed += (s, e) =>
                    {
                        WindowsController(cancellationToken).ConfigureAwait(false);
                    };

                    IntPtr hWnd = WindowNative.GetWindowHandle(ActiveWindow);
                    WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
                    AppWindow appWindow = AppWindow.GetFromWindowId(wndId);
                    if (appWindow != null)
                    {
                        appWindow.MoveAndResize(new Windows.Graphics.RectInt32(600, 600, 420, 600));
                        appWindow.MoveInZOrderAtTop();
                        appWindow.TitleBar.ExtendsContentIntoTitleBar = true;
                        appWindow.Title = "UserCentral";
                        var titleBar = appWindow.TitleBar;
                        titleBar.ExtendsContentIntoTitleBar = true;
                        var btnColor = Colors.Transparent;
                        titleBar.BackgroundColor = btnColor;
                        titleBar.ForegroundColor = btnColor;
                        titleBar.ButtonBackgroundColor = btnColor;
                        titleBar.ButtonInactiveBackgroundColor = btnColor;
                        appWindow.SetPresenter(Microsoft.UI.Windowing.AppWindowPresenterKind.CompactOverlay);
                        appWindow.SetIcon("ms-appx:///logo.ico");
                    }

                    ActiveWindow.Activate();
                    Windowing.Center(ActiveWindow);

                    return Task.CompletedTask;
                }



                if (AuthService.CurrentUser is not null && AuthService.IsUserAuthenticated)
                {
                    //load dependencies applicaton wide. 
                    if (Directory.Exists(UserDataManager.CoreFolderPath))
                    {
                        App.Current.Services = App.Current.ConfigureServices();
                    }


                    var userExist = Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, AuthService.CurrentUser?.Username);
                    // very odd bug: if AuthService.CurrentUser is in UsrCore file, but folders are missing. 
                    if (!Directory.Exists(userExist))
                    {
                        UserFolderManager.CreateUserFolders(new User()
                        {
                            Id = Guid.NewGuid(),
                            Username = AuthService.CurrentUser.Username,
                            IsFirstLaunch = false,
                            UserSettings = null
                        });
                        // need to load a default settings for user because everything was gone and now need to recreate. HUM go to setup again ?  this is quick fix. 
                        AppService.AppSettings = new Settings(true).Self;
                    }

                    // double authenticate, and then create all databases and then insert setting (is not exists in db from this service AppSettings)
                    CheckNormal(AuthService.CurrentUser.Username);

                    App.Current.m_window = new MainWindow();
                    Windowing.Center(App.Current.m_window!);
                    IntPtr hWnd = WindowNative.GetWindowHandle(App.Current.m_window);
                    App.Current.m_window.Activate();
                    App.Current.m_window.AppWindow.MoveInZOrderAtTop();


                    if (Windowing.IsWindowVisible(hWnd))
                    {
                        await Task.Delay(1000);
                        if (AuthService.IsUserAuthenticated)
                        {
                            IMessenger messenger = App.GetService<IMessenger>();
                            messenger?.Send(new Message_Settings_Actions($"Welcome {AuthService.CurrentUser.Username} to our FireBrowser", EnumMessageStatus.Login));
                        }
                    }
                    // cancel all recursions and send back to base.launched.app 
                    CancellationTokenSource cancel = new CancellationTokenSource();
                    CancellationToken = cancellationToken = cancel.Token;
                    cancel.Cancel();

                }

            }
            catch (Exception e)
            {
                CancellationTokenSource cancel = new CancellationTokenSource();
                cancellationToken = cancel.Token;
                cancel.Cancel();

                return Task.FromException<CancellationToken>(e);
                throw;
            }
            // doesn't effect other calls, but you may use to get to the final objective of showing mainwindow with an authenicated user. 
            return Task.FromCanceled(cancellationToken);
        }
        public static string GetUsernameFromCoreFolderPath(string coreFolderPath, string userName = null)
        {
            try
            {
                var users = JsonSerializer.Deserialize<List<FireBrowserWinUi3MultiCore.User>>(File.ReadAllText(Path.Combine(coreFolderPath, "UsrCore.json")));

                return users?.FirstOrDefault(u => !string.IsNullOrWhiteSpace(u.Username) && (userName == null || u.Username.Equals(userName, StringComparison.CurrentCultureIgnoreCase)))?.Username;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading UsrCore.json: {ex.Message}");
            }

            return null;
        }

        static async void CheckNormal(string userName = null)
        {
            string coreFolderPath = UserDataManager.CoreFolderPath;
            string username = GetUsernameFromCoreFolderPath(coreFolderPath, userName);

            if (username != null)
            {
                AuthService.Authenticate(username);
                DatabaseServices dbServer = new DatabaseServices();

                try
                {
                    await dbServer.DatabaseCreationValidation();
                    await dbServer.InsertUserSettings();

                }
                catch (Exception ex)
                {
                    ExceptionLogger.LogException(ex);
                    Console.WriteLine($"Creating Settings for user already exists\n {ex.Message}");
                }
            }
        }

        static public async void CreateNewUsersSettings()
        {

            ActiveWindow = new UserSettings();
            ActiveWindow.Closed += async (s, e) =>
            {

                try
                {
                    SettingsActions settingsActions = new SettingsActions(AuthService.NewCreatedUser?.Username);
                    if (!File.Exists(Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, AuthService.NewCreatedUser?.Username, "Settings", "Settings.db")))
                    {
                        await settingsActions.SettingsContext.Database.MigrateAsync();
                    }
                    if (File.Exists(Path.Combine(UserDataManager.CoreFolderPath, UserDataManager.UsersFolderPath, AuthService.NewCreatedUser?.Username, "Settings", "Settings.db")))
                        await settingsActions.SettingsContext.Database.CanConnectAsync();

                    if (await settingsActions.GetSettingsAsync() is null)
                    {
                        await settingsActions.InsertUserSettingsAsync(AppService.AppSettings);
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogger.LogException(ex);
                    Console.WriteLine($"Error in Creating Settings Database: {ex.Message}");
                }
                finally
                {
                    AuthService.NewCreatedUser = null;
                }


            };

            IntPtr hWnd = WindowNative.GetWindowHandle(ActiveWindow);
            WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            AppWindow appWindow = AppWindow.GetFromWindowId(wndId);

            if (appWindow != null)
            {
                SizeInt32? desktop = await Windowing.SizeWindow();
                appWindow.MoveAndResize(new RectInt32(desktop.Value.Height / 2, desktop.Value.Width / 2, (int)(desktop?.Width * .60), (int)(desktop?.Height * .60)));
                appWindow.MoveInZOrderAtTop();
                appWindow.TitleBar.ExtendsContentIntoTitleBar = true;
                appWindow.Title = "Settings for : " + AuthService.NewCreatedUser.Username;
                var titleBar = appWindow.TitleBar;
                titleBar.ExtendsContentIntoTitleBar = true;
                var btnColor = Colors.Transparent;
                titleBar.BackgroundColor = btnColor;
                titleBar.ForegroundColor = btnColor;
                titleBar.ButtonBackgroundColor = btnColor;
                titleBar.ButtonInactiveBackgroundColor = btnColor;
                appWindow.SetPresenter(Microsoft.UI.Windowing.AppWindowPresenterKind.Overlapped);

            }
            Windowing.Center(ActiveWindow);
            appWindow.ShowOnceWithRequestedStartupState(); ;

        }
    }
}
