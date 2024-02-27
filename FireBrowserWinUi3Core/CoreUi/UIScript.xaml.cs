using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace FireBrowserWinUi3Core.CoreUi;

public sealed partial class UIScript : ContentDialog
{
    public UIScript() => InitializeComponent();

    public static async Task ShowDialog(string title, string content, XamlRoot root)
    {
        ContentDialog dialog = new()
        {
            Title = title,
            XamlRoot = root,
            Content = content,
            PrimaryButtonText = "Okay",
            DefaultButton = ContentDialogButton.Primary
        };

        await dialog.ShowAsync();
    }
}