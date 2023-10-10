using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FireBrowserWinUi3.Setup
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SetupInit : Page
    {
        public SetupInit()
        {
            this.InitializeComponent();
        }

        private string _introMessage = @"
• Seamless browsing experience.

• One-click access to favorite websites and a built-in favorites organizer.

• Immersive full-screen mode.

• Prioritizes user convenience.

• Caters to users seeking a user-friendly web browser with advanced features.
";

        private void Setup_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SetupUser));
        }
    }
}
