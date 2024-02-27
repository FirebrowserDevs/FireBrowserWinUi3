using CommunityToolkit.Mvvm.ComponentModel;

namespace FireBrowserWinUi3Core.ViewModel
{
    public partial class TabViewItemViewModel : ObservableObject
    {
        [ObservableProperty] public bool _IsTooltipEnabled;
    }
}
