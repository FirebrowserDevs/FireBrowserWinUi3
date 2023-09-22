using CommunityToolkit.Mvvm.ComponentModel;
using FireBrowserBusinessCore.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FireBrowserBusiness.Controls
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FireBrowserTabView : TabView
    {
        public FireBrowserTabView()
        {
            this.InitializeComponent();
            ViewModel = new FireBrowserTabViewViewModel()
            {
                Style = (Style)Application.Current.Resources["DefaultTabViewStyle"]
            };
        }

        public FireBrowserTabViewViewModel ViewModel { get; set; } = new FireBrowserTabViewViewModel();

        public partial class FireBrowserTabViewViewModel : ObservableObject
        {
            [ObservableProperty] private Style style;
        }

        public Settings.UILayout Mode
        {
            get => (Settings.UILayout)GetValue(ModeProperty);
            set
            {
                ViewModel.Style = value switch
                {
                    Settings.UILayout.Modern => (Style)Application.Current.Resources["DefaultTabViewStyle"],
                    Settings.UILayout.Vertical => (Style)Application.Current.Resources["VerticalTabViewStyle"],
                    _ => (Style)Application.Current.Resources["DefaultTabViewStyle"]
                };

                SetValue(ModeProperty, value);
            }
        }


        public static DependencyProperty ModeProperty = DependencyProperty.Register(
        nameof(Mode),
        typeof(Settings.UILayout),
        typeof(FireBrowserTabView),
        null);
    }
}
