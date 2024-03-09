using FireBrowserWinUi3.Services;
using FireBrowserWinUi3.Services.ViewModels;
using FireBrowserWinUi3Core.Helpers;
using FireBrowserWinUi3Exceptions;
using FireBrowserWinUi3MultiCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Path = System.IO.Path;

namespace FireBrowserWinUi3;
public partial class App : Application
{
    public new static App Current => (App)Application.Current;

    #region DependencyInjection

    public IServiceProvider Services { get; set; }

    public static T GetService<T>() where T : class
    {
        if ((App.Current as App)!.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }

        return service;
    }
    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        //services.AddDbContext<FireBrowserDataCore.HistoryContext>();
        services.AddSingleton<DownloadService>();
        services.AddTransient<DownloadsViewModel>();
        services.AddSingleton<SettingsService>();
        return services.BuildServiceProvider();
    }
    #endregion
    public App()
    {


        this.InitializeComponent();

        FireBrowserWinUi3Navigator.TLD.LoadKnownDomainsAsync();

        System.Environment.SetEnvironmentVariable("WEBVIEW2_USE_VISUAL_HOSTING_FOR_OWNED_WINDOWS", "1");
    }


    public static string GetUsernameFromCoreFolderPath(string coreFolderPath)
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

    public async void CheckNormal()
    {
        string coreFolderPath = UserDataManager.CoreFolderPath;
        string username = GetUsernameFromCoreFolderPath(coreFolderPath);

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
                        AuthService.Authenticate(username);
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
                CheckNormal();
            }

            // Activate the window after evaluating the URL and handling respective cases
            m_window = new MainWindow();
        }

        // Activate the window outside of conditional blocks
        m_window.Activate();
    }

    public Window m_window;
}