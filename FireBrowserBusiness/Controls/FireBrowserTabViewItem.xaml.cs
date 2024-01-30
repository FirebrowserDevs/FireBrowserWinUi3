using FireBrowserWinUi3.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;

namespace FireBrowserBusiness.Controls;
public sealed partial class FireBrowserTabViewItem : TabViewItem
{
    public FireBrowserTabViewItem() => InitializeComponent();
    public bool IsViewOpen { get; private set; }
    public BitmapImage BitViewWebContent { get; set; }
    public string Value
    {
        get => (string)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public static DependencyProperty ValueProperty = DependencyProperty.Register(
    nameof(Value),
    typeof(string),
    typeof(FireBrowserTabViewItem),
    null);

    private void TabViewItem_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        MainWindow win = (Window)(Application.Current as App).m_window as MainWindow;
        if (win?.TabViewContainer.SelectedItem is FireBrowserTabViewItem tab)
        {
            if (win?.TabContent.Content is WebContent web)
            {
                // test to see if exists, hence they is a timeout of 2400 millseconds to load the page
                // this also allow NOT to show if it's a newTab aswell...dizzle. 2024-01-30
                // need to work on ie:  if 2 tabs are present no matter where the mouse enters on the TabContainer it will show the SelectedTabs View - weird
                // jarno I DIDN't use an exitof the mouse event because that was I believe you weird or itchy ticking of the flyout. 
                if (web.PictureWebElement is BitmapImage)
                {
                    ImgTabViewItem.Source = web.PictureWebElement;
                    Flyout.GetAttachedFlyout(TabViewItem).ShowAt(TabViewItem);
                }

            }

        }



    }


}