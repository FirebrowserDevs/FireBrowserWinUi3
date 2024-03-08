using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FireBrowserWinUi3Setup
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SetupFinish : Page
    {
        private int countdownSeconds = 5;
        private DispatcherTimer timer;
        public SetupFinish()
        {
            this.InitializeComponent();
            this.Loaded += SetupFinish_Loaded;
        }

        private void SetupFinish_Loaded(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;

            // Start the timer
            timer.Start();
        }

        private async void Timer_Tick(object sender, object e)
        {
            countdownSeconds--;

            if (countdownSeconds <= 0)
            {
                Microsoft.Windows.AppLifecycle.AppInstance.Restart("");
            }
            else
            {

            }
        }
    }
}
