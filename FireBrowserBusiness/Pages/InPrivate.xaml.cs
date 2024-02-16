using FireBrowserMultiCore;
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
        JavToggle.IsOn = userSettings.DisableJavaScript == "1" ? true : false;
        WebToggle.IsOn = userSettings.DisableWebMess == "1" ? true : false;
    }

    private void ToggleSwitch_Toggled(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            string autoSettingValue = toggleSwitch.IsOn ? "1" : "0";

            userSettings.DisableJavaScript = autoSettingValue;

            UserFolderManager.TempSaveSettings("Private", userSettings);
        }
    }

    private void ToggleSwitch_Toggled_1(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            string autoSettingValue = toggleSwitch.IsOn ? "1" : "0";

            userSettings.DisableWebMess = autoSettingValue;

            UserFolderManager.TempSaveSettings("Private", userSettings);
        }
    }
}