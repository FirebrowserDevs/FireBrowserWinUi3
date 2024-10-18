using CommunityToolkit.Mvvm.Messaging;
using FireBrowserWinUi3.Services;
using FireBrowserWinUi3.Services.ViewModels;
using FireBrowserWinUi3.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Path = System.IO.Path;

namespace FireBrowserWinUi3;
public partial class App : Application
{
    string changeUsernameFilePath = Path.Combine(Path.GetTempPath(), "changeusername.json");
    public new static App Current => (App)Application.Current;

    #region DependencyInjection

    public IServiceProvider Services { get; set; }

    public static T GetService<T>() where T : class
    {
        if (App.Current is not App app || App.Current.Services is null)
        {
            throw new NullReferenceException("Application or Services are not properly initialized.");
        }

        if (App.Current.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }

        return service;
    }
    public IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<WeakReferenceMessenger>();
        services.AddSingleton<IMessenger, WeakReferenceMessenger>(provider =>
            provider.GetRequiredService<WeakReferenceMessenger>());
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
        this.UnhandledException += Current_UnhandledException;
        _ = FireBrowserWinUi3Navigator.TLD.LoadKnownDomainsAsync().ConfigureAwait(false);

        System.Environment.SetEnvironmentVariable("WEBVIEW2_USE_VISUAL_HOSTING_FOR_OWNED_WINDOWS", "1");
    }

    private void Current_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        if (!AppService.IsAppGoingToClose)
            FireBrowserWinUi3Exceptions.ExceptionLogger.LogException(e.Exception);
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



    private string AzureStorage { get; } = "DefaultEndpointsProtocol=https;AccountName=strorelearn;AccountKey=0pt8CYqrqXUluQE3/60q8wobkmYznb9ovHIzztGVOzNxlSa+U8NlY74uwfggd5DfTmGORBLtXpeKEvDYh2ynfQ==;EndpointSuffix=core.windows.net";
    
    protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        AppService.CancellationToken = CancellationToken.None;

        await AppService.WindowsController(AppService.CancellationToken);

        while (!AppService.CancellationToken.IsCancellationRequested)
        {
            await Task.Delay(1500);
        }

        if (AppService.IsAppGoingToClose == true)
            base.Exit();
        else
            base.OnLaunched(args);

        Windows.Storage.ApplicationData.Current.LocalSettings.Values["AzureStorageConnectionString"] = AzureStorage;
        
        //var az = new AzBackupService(AzureStorage, "storelean", "FireBackups", new() { Id = Guid.NewGuid(), Username = "Admin", IsFirstLaunch = false});

    }

    public Window m_window;
}
