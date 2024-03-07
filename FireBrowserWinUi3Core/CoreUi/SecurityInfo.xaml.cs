using ColorCode.Compilation.Languages;
using CommunityToolkit.Common.Parsers;
using FireBrowserWinUi3Core.Models;
using FireBrowserWinUi3Core.ViewModel;
using Microsoft.UI.Xaml.Controls;

namespace FireBrowserWinUi3Core.CoreUi;

public sealed partial class SecurityInfo : Flyout
{
    public Passer passer;

    public SecurityInfo()
    {
        this.InitializeComponent();

        Data();
    }

    public void Data()
    {
     
    }
}