using FireBrowserBusiness;
using FireBrowserBusinessCore;
using FireBrowserMultiCore;
using FireBrowserWinUi3.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FireBrowserWinUi3.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ModulesInstaller : Page
    {
        public ModulesInstaller()
        {
            this.InitializeComponent();

        }

        private async Task LoadModulesAsync()
        {
            FireBrowserMultiCore.User user = AuthService.CurrentUser;
            string username = user.Username;
            string databasePath = Path.Combine(
                UserDataManager.CoreFolderPath,
                UserDataManager.UsersFolderPath,
                username,
                "Modules"
            );
            // Directory where module DLLs are stored
            if (!Directory.Exists(databasePath))
            {
                // Handle directory not found
                return;
            }

            string[] moduleFiles = Directory.GetFiles(databasePath, "*.dll");
            foreach (string file in moduleFiles)
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(file);
                    var moduleTypes = assembly.GetTypes()
                        .Where(t => typeof(IModule).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                    foreach (Type type in moduleTypes)
                    {
                        IModule module = Activator.CreateInstance(type) as IModule;
                        UIElement moduleUI = await module.GetModuleUIAsync(); // Await the async method

                        if (moduleUI != null)
                        {
                            test.Content = moduleUI;
                        }
                    }
                }
                catch (Exception ex)
                {
                    var window = (Application.Current as App)?.m_window as MainWindow;
                    UI quickConfigurationDialog = new()
                    {
                        XamlRoot = window.Content.XamlRoot,
                        Content = ex.Message + "Miss Configuration"
                    };

                    await quickConfigurationDialog.ShowAsync();
                }
            }
        }


        private async void tabControlMain_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadModulesAsync();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
