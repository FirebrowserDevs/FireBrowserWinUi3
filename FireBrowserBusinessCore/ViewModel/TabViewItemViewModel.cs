using CommunityToolkit.Mvvm.ComponentModel;

namespace FireBrowserBusinessCore.ViewModel
{
    public partial class TabViewItemViewModel : ObservableObject
    {
        [ObservableProperty] public bool _IsTooltipEnabled;
    }
}
