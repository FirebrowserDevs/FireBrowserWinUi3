using FireBrowserWinUi3.Services;
using FireBrowserWinUi3MultiCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Media.SpeechSynthesis;

namespace FireBrowserWinUi3.Pages.SettingsPages;
public sealed partial class SettingsAccess : Page
{
    bool isMode;

    SettingsService SettingsService { get; set; }

    public SettingsAccess()
    {
        this.InitializeComponent();
        SettingsService = App.GetService<SettingsService>();
        LoadUserDataAndSettings();
        id();
        ShowSupportedLanguagesAndGenders();
    }

    private void ShowSupportedLanguagesAndGenders()
    {
        var synthesizer = new SpeechSynthesizer();
        var allVoices = SpeechSynthesizer.AllVoices;

        // Populate language ComboBox
        var distinctLanguages = allVoices
            .Select(voice => voice.Language)
            .Distinct()
            .ToList();

        Langue.Items.Clear();
        foreach (var language in distinctLanguages)
        {
            Langue.Items.Add(language);
        }

        string storedLanguage = SettingsService.CoreSettings.Lang;
        if (!string.IsNullOrEmpty(storedLanguage) && distinctLanguages.Contains(storedLanguage))
        {
            Langue.SelectedItem = storedLanguage;
        }
        else if (Langue.Items.Count > 0)
        {
            Langue.SelectedIndex = 0;
        }

        // Populate gender ComboBox
        var distinctGenders = allVoices
            .Select(voice => voice.Gender.ToString())
            .Distinct()
            .ToList();

        Gender.Items.Clear();
        foreach (var gender in distinctGenders)
        {
            Gender.Items.Add(gender);
        }

        string storedGender = SettingsService.CoreSettings.Gender;
        if (!string.IsNullOrEmpty(storedGender) && distinctGenders.Contains(storedGender))
        {
            Gender.SelectedItem = storedGender;
        }
        else if (Gender.Items.Count > 0)
        {
            Gender.SelectedIndex = 0;
        }

        // Debug output of available voices
        foreach (var voice in allVoices)
        {
            var language = voice.Language;
            var displayName = voice.DisplayName;
            var gender = voice.Gender;

            Debug.WriteLine($"Voice: {displayName}, Language: {language}, Gender: {gender}{Environment.NewLine}");
        }
    }


    private void LoadUserDataAndSettings()
    {
        Langue.SelectedValue = SettingsService.CoreSettings.Lang;
        Logger.SelectedValue = SettingsService.CoreSettings.ExceptionLog;
        bool isMode = (bool)SettingsService.CoreSettings.LightMode;
        LiteMode.IsOn = isMode;
    }
    private async void LiteMode_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            isMode = toggleSwitch.IsOn;
            var autoValue = isMode;


            if (AuthService.CurrentUser != null)
            {
                // Update the "Auto" setting for the current user
                SettingsService.CoreSettings.LightMode = autoValue;

                // Save the modified settings back to the user's settings file
                await SettingsService.SaveChangesToSettings(AuthService.CurrentUser, SettingsService.CoreSettings);
            }
            else
            {
                // Handle the case when there is no authenticated user.
            }
        }
    }

    private async void Langue_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Check if there is a valid selection (protect against nulls)
        if (e.AddedItems.Count == 0 || e.AddedItems[0] == null) return;

        // Get the selected language from the ComboBox
        string selectedLanguage = e.AddedItems[0].ToString();

        // Since known languages come from the system, we don't need to hard-code the switch, just assign the selection
        SettingsService.CoreSettings.Lang = selectedLanguage;

        try
        {
            // Save the modified settings back to the user's settings file
            await SettingsService.SaveChangesToSettings(AuthService.CurrentUser, SettingsService.CoreSettings);

            // Optionally, notify the user that the language has been successfully updated
            Debug.WriteLine($"Language setting updated to: {selectedLanguage}");
        }
        catch (Exception ex)
        {
            // Handle any errors that might occur during the settings save
            Debug.WriteLine($"Error updating language setting: {ex.Message}");
        }
    }

    private async void id()
    {
        var startup = await StartupTask.GetAsync("FireBrowserWinUiStartUp");
        UpdateToggleState(startup.State);
    }

    private void UpdateToggleState(StartupTaskState state)
    {
        LaunchOnStartupToggle.IsEnabled = true;
        LaunchOnStartupToggle.IsChecked = state switch
        {
            StartupTaskState.Enabled => true,
            StartupTaskState.Disabled => false,
            StartupTaskState.DisabledByUser => false,
            _ => LaunchOnStartupToggle.IsEnabled = false
        };
    }
    private async Task ToggleLaunchOnStartup(bool enable)
    {
        var startup = await StartupTask.GetAsync("FireBrowserWinUiStartUp");

        switch (startup.State)
        {
            case StartupTaskState.Enabled when !enable:
                startup.Disable();
                break;
            case StartupTaskState.Disabled when enable:
                var updatedState = await startup.RequestEnableAsync();
                UpdateToggleState(updatedState);
                break;
            case StartupTaskState.DisabledByUser when enable:
                ContentDialog cs = new ContentDialog();
                cs.Title = "Unable to change state of startup task via the application";
                cs.Content = "Enable via Startup tab on Task Manager (Ctrl+Shift+Esc)";
                cs.PrimaryButtonText = "OK";
                await cs.ShowAsync();
                break;
            default:
                ContentDialog cs2 = new ContentDialog();
                cs2.Title = "Unable to change state of startup task";
                cs2.PrimaryButtonText = "OK";
                await cs2.ShowAsync();
                break;
        }
    }

    private async void LaunchOnStartupToggle_Click(object sender, RoutedEventArgs e)
    {
        await ToggleLaunchOnStartup(LaunchOnStartupToggle.IsChecked ?? false);
    }

    private async void Logger_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        string selection = e.AddedItems[0].ToString();
        string langSetting = selection switch
        {
            "Low" => "Low",
            "High" => "High",
            _ => throw new ArgumentException("Invalid selection")
        };

        // Update the "Auto" setting for the current user
        SettingsService.CoreSettings.ExceptionLog = langSetting;

        // Save the modified settings back to the user's settings file
        await SettingsService.SaveChangesToSettings(AuthService.CurrentUser, SettingsService.CoreSettings);
    }

    private void WelcomeMesg_Toggled(object sender, RoutedEventArgs e)
    {

    }

    private async void Gender_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Check if there is a valid selection (protect against nulls)
        if (e.AddedItems.Count == 0 || e.AddedItems[0] == null) return;

        // Get the selected gender from the ComboBox
        string selectedGender = e.AddedItems[0].ToString();

        // Save the selected gender in the settings
        SettingsService.CoreSettings.Gender = selectedGender;

        try
        {
            // Save the modified settings back to the user's settings file
            await SettingsService.SaveChangesToSettings(AuthService.CurrentUser, SettingsService.CoreSettings);

            // Optionally notify the user that the gender has been updated
            Debug.WriteLine($"Gender setting updated to: {selectedGender}");
        }
        catch (Exception ex)
        {
            // Handle any errors during the settings save
            Debug.WriteLine($"Error updating gender setting: {ex.Message}");
        }
    }
}
