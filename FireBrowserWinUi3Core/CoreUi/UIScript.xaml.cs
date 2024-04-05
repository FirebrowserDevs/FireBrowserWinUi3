using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace FireBrowserWinUi3Core.CoreUi;

public sealed partial class UIScript : ContentDialog
{
    public UIScript(string title, string content, XamlRoot root)
    {
        InitializeComponent();
        this.Title = title;
        this.XamlRoot = root;
        this.Content = content;
        this.PrimaryButtonText = "Okay";
        DefaultButton = ContentDialogButton.Primary;
    } 
}