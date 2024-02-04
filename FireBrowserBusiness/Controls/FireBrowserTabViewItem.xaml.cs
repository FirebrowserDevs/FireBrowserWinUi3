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

        if ((sender as FireBrowserBusiness.Controls.FireBrowserTabViewItem).IsSelected)
            if (win?.TabViewContainer.SelectedItem is FireBrowserTabViewItem tab)
            {
                if (win?.TabContent.Content is WebContent web)
                {
                    if (web.PictureWebElement is BitmapImage)
                    {
                        ImgTabViewItem.Source = web.PictureWebElement;
                        Flyout.GetAttachedFlyout(TabViewItem).ShowAt(TabViewItem);
                    }

                }
            }
    }
}