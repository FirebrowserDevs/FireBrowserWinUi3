using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;

namespace FireBrowserBusinessCore.ViewModel;

public partial class ToolbarViewModel : ObservableObject
{
    [ObservableProperty] public bool canRefresh;
    [ObservableProperty] public bool canGoBack;
    [ObservableProperty] public bool canGoForward;
    [ObservableProperty] public string currentAddress;
    [ObservableProperty] public string securityIcon;
    [ObservableProperty] public string securityIcontext;
    [ObservableProperty] public string securitytext;
    [ObservableProperty] public string securitytype;
    [ObservableProperty] public Visibility homeButtonVisibility;
}