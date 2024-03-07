using FireBrowserWinUi3MultiCore;
using Microsoft.UI.Xaml.Controls;

namespace FireBrowserWinUi3.Pages;
public sealed partial class InPrivate : Page
{
    Settings userSettings = UserFolderManager.TempLoadPrivate("Private");

    public InPrivate()
    {
        this.InitializeComponent();
        Init();
    }

    public void Init()
    {
        JavToggle.IsOn = userSettings.DisableJavaScript;
        WebToggle.IsOn = userSettings.DisableWebMess;
    }

    private void ToggleSwitch_Toggled(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            var autoSettingValue = toggleSwitch.IsOn;

            userSettings.DisableJavaScript = autoSettingValue;

            UserFolderManager.TempSaveSettings("Private", userSettings);
        }
    }

    private void ToggleSwitch_Toggled_1(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            var autoSettingValue = toggleSwitch.IsOn;

            userSettings.DisableWebMess = autoSettingValue;

            UserFolderManager.TempSaveSettings("Private", userSettings);
        }
    }
}