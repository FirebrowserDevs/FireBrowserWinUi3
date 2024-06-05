using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.Web.WebView2.Core;
using System;
using Windows.Foundation;

namespace FireBrowserWinUi3.Controls
{
    public sealed partial class PopUpView : UserControl
    {
        private bool isDragging;
        private Point clickPosition;

        public PopUpView()
        {
            this.InitializeComponent();
        }

        public void SetSource(Uri uri)
        {
            webView.Source = uri;
        }

        public void Show()
        {
            this.Visibility = Visibility.Visible;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
            if (this.Parent is Panel parentPanel)
            {
                parentPanel.Children.Remove(this);
                webView.CoreWebView2.Stop();
                webView.Close();
            }
        }

        private void Header_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            isDragging = true;
            clickPosition = e.GetCurrentPoint(this).Position;
            this.CapturePointer(e.Pointer);
        }

        private void Header_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (isDragging)
            {
                var mainWindow = (Application.Current as App)?.m_window as MainWindow;
                var transform = this.TransformToVisual(mainWindow.Content);
                Point screenCoords = e.GetCurrentPoint(mainWindow.Content).Position;
                Point relativeCoords = transform.TransformPoint(screenCoords);

                Canvas.SetLeft(this, relativeCoords.X - clickPosition.X);
                Canvas.SetTop(this, relativeCoords.Y - clickPosition.Y);
            }
        }

        private void Header_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            isDragging = false;
            this.ReleasePointerCaptures();
        }
    }
}
