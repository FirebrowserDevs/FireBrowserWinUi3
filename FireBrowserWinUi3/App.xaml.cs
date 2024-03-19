using CommunityToolkit.Mvvm.Messaging;
using FireBrowserWinUi3.Services;
using FireBrowserWinUi3.Services.Messages;
using FireBrowserWinUi3.Services.ViewModels;
using FireBrowserWinUi3.ViewModels;
using FireBrowserWinUi3Core.Helpers;
using FireBrowserWinUi3Exceptions;
using FireBrowserWinUi3MultiCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using WinRT.Interop;
using Path = System.IO.Path;

namespace FireBrowserWinUi3;
public partial class App : Application
{

    string changeUsernameFilePath = Path.Combine(Path.GetTempPath(), "changeusername.json");
    public new static App Current => (App)Application.Current;

    #region DependencyInjection

    public IServiceProvider Services { get; private set; }

    public static T GetService<T>() where T : class
    {
        if (App.Current == null || !(App.Current is App app) || app.Services == null)
        {
            throw new NullReferenceException("Application or Services are not properly initialized.");
        }

        if (app.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }

        return service;
    }

    private void InitializeServices()
    {
        Services = ConfigureServices();
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<WeakReferenceMessenger>();
        services.AddSingleton<IMessenger, WeakReferenceMessenger>(provider =>
            provider.GetRequiredService<WeakReferenceMessenger>());
        //services.AddDbContext<FireBrowserDataCore.HistoryContext>();
        services.AddSingleton<DownloadService>();
        services.AddTransient<DownloadsViewModel>();

        services.AddSingleton<SettingsService>();

        services.AddTransient<HomeViewModel>();
        services.AddTransient<MainWindowViewModel>();

        return services.BuildServiceProvider();
    }

    #endregion

    public App()
    {
        this.InitializeComponent();
        InitializeServices();
        FireBrowserWinUi3Navigator.TLD.LoadKnownDomainsAsync();

        System.Environment.SetEnvironmentVariable("WEBVIEW2_USE_VISUAL_HOSTING_FOR_OWNED_WINDOWS", "1");
    }


    public static string GetUsernameFromCoreFolderPath(string coreFolderPath, string userName = null)
    {
        try
        {
            string usrCoreFilePath = Path.Combine(coreFolderPath, "UsrCore.json");

            if (File.Exists(usrCoreFilePath))
            {
                string jsonContent = File.ReadAllText(usrCoreFilePath);
                var users = JsonSerializer.Deserialize<List<FireBrowserWinUi3MultiCore.User>>(jsonContent);

                if (users?.Count > 0 && !string.IsNullOrWhiteSpace(users[0].Username))
                {
                    if (userName != null)
                    {
                        return users.Single(t => t.Username.Equals(userName, StringComparison.CurrentCultureIgnoreCase)).Username;
                    }
                    return users[0].Username;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading UsrCore.json: {ex.Message}");
        }

        return null;
    }

    public async void CheckNormal(string userName = null)
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
                if (Directory.Exists(UserDataManager.CoreFolderPath))
                {
                    Services = ConfigureServices();

                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                Console.WriteLine($"Creating Settings for user already exists\n {ex.Message}");
            }


        }
    }


    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        if (!Directory.Exists(UserDataManager.CoreFolderPath))
        {
            // The "FireBrowserUserCore" folder does not exist, so proceed with application setup behavior.
            m_window = new SetupWindow();
        }
        else
        {
            if (File.Exists(changeUsernameFilePath))
            {
                m_window = new ChangeUsernameCore();
            }
            else
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

                        // Extract the username after 'firebrowserwinuifireuser://'
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
                else
                {
                    m_window = new UserCentral();
                    IntPtr hWnd = WindowNative.GetWindowHandle(m_window);
                    WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
                    AppWindow appWindow = AppWindow.GetFromWindowId(wndId);
                    if (appWindow != null) { 
                        appWindow.MoveAndResize(new Windows.Graphics.RectInt32(500, 500, 420, 500));
                        appWindow.MoveInZOrderAtTop();
                        appWindow.TitleBar.ExtendsContentIntoTitleBar = true;
                        appWindow.SetPresenter(Microsoft.UI.Windowing.AppWindowPresenterKind.CompactOverlay);
                        appWindow.SetIcon("ms-appx:///logo.ico");
                    }
                    Windowing.Center(m_window);
                    m_window.Activate();
                    return; 
                }

                
            }
            m_window = new MainWindow();
            if (AuthService.IsUserAuthenticated)
            {
                IMessenger messenger = App.GetService<IMessenger>();
                messenger?.Send(new Message_Settings_Actions($"Welcome {AuthService.CurrentUser.Username} to our FireBrowser", EnumMessageStatus.Login));
            }
        }
        // Activate the window outside of conditional blocks
        m_window.Activate();
    }

    public Window m_window;
}