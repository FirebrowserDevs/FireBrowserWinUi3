using FireBrowserWinUi3.Services;
using FireBrowserWinUi3MultiCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FireBrowserWinUi3.Pages.SettingsPages;

public sealed partial class SettingsPrivacy : Page
{
    SettingsService SettingsService { get; set; }

    public SettingsPrivacy()
    {
        SettingsService = App.GetService<SettingsService>();
        this.InitializeComponent();
        Stack();
    }

    public void Stack()
    {
        DisableJavaScriptToggle.IsOn = SettingsService.CoreSettings.DisableJavaScript;
        DisablWebMessFillToggle.IsOn = SettingsService.CoreSettings.DisableWebMess;
        DisableGenaralAutoFillToggle.IsOn = SettingsService.CoreSettings.DisableGenAutoFill;
        PasswordWebMessFillToggle.IsOn = SettingsService.CoreSettings.DisablePassSave;
    }

    private async void DisableJavaScriptToggle_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            var autoSettingValue = toggleSwitch.IsOn;

            SettingsService.CoreSettings.DisableJavaScript = autoSettingValue;

            await SettingsService.SaveChangesToSettings(AuthService.CurrentUser, SettingsService.CoreSettings);
        }
    }

    private async void DisableGenaralAutoFillToggle_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            var autoSettingValue = toggleSwitch.IsOn;

            SettingsService.CoreSettings.DisableGenAutoFill = autoSettingValue;

            await SettingsService.SaveChangesToSettings(AuthService.CurrentUser, SettingsService.CoreSettings);
        }
    }

    private async void DisablWebMessFillToggle_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            var autoSettingValue = toggleSwitch.IsOn;

            SettingsService.CoreSettings.DisableWebMess = autoSettingValue;

            await SettingsService.SaveChangesToSettings(AuthService.CurrentUser, SettingsService.CoreSettings);
        }
    }

    private async void PasswordWebMessFillToggle_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            var autoSettingValue = toggleSwitch.IsOn;

            SettingsService.CoreSettings.DisablePassSave = autoSettingValue;

            await SettingsService.SaveChangesToSettings(AuthService.CurrentUser, SettingsService.CoreSettings);
        }
    }

    private void Confirm_Click(object sender, RoutedEventArgs e)
    {
        Microsoft.Windows.AppLifecycle.AppInstance.Restart("");
    }

    private void CamPermission_Toggled(object sender, RoutedEventArgs e)
    {

    }
}