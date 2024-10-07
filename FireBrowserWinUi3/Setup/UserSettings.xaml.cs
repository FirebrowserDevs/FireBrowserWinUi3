using FireBrowserWinUi3.Services;
using FireBrowserWinUi3MultiCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FireBrowserWinUi3.Setup
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UserSettings : Window
    {
        public string UserName { get; internal set; } = AuthService.NewCreatedUser?.Username ?? AuthService.CurrentUser?.Username ?? "User Settings";
        public UserSettings()
        {
            this.InitializeComponent();
            AppService.AppSettings = new FireBrowserWinUi3MultiCore.Settings(true).Self;

        }

        public int previousSelectedIndex { get; private set; }

        private void SelectorBar2_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
        {
            SelectorBarItem selectedItem = sender.SelectedItem;
            int currentSelectedIndex = sender.Items.IndexOf(selectedItem);

            System.Type pageType;

            switch (currentSelectedIndex)
            {

                case 0:
                    pageType = typeof(SetupUi);
                    break;
                case 1:
                    pageType = typeof(SetupAlgemeen);
                    break;
                case 2:
                    pageType = typeof(SetupPrivacy);
                    break;
                case 3:
                    pageType = typeof(SetupAccess);
                    break;
                case 4:
                    pageType = typeof(SetupWebView);
                    break;
                default:
                    pageType = typeof(SetupFinish);
                    break;
            }

            var slideNavigationTransitionEffect = currentSelectedIndex - previousSelectedIndex > 0 ? SlideNavigationTransitionEffect.FromRight : SlideNavigationTransitionEffect.FromLeft;

            ContentFrame.Navigate(pageType, null, new SlideNavigationTransitionInfo() { Effect = slideNavigationTransitionEffect });

            previousSelectedIndex = currentSelectedIndex;
        }
    }
}
