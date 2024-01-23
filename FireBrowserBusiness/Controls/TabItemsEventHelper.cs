using FireBrowserBusiness.Controls;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Input;
using System;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;
using System.Threading.Tasks;

namespace FireBrowserWinUi3.Controls
{
    public class TabItemEventsHelper
    {
        private bool isFlyoutOpen = false;
        private Flyout tabsHover;
        private DispatcherTimer flyoutCloseTimer;
        public TabItemEventsHelper(Flyout tabsHover)
        {
            this.tabsHover = tabsHover;
        }

        public void AttachEventHandlers(FireBrowserTabViewItem newItem)
        {
            newItem.PointerPressed += OnPointerPressed;
            newItem.PointerMoved += OnPointerMoved;
            newItem.PointerEntered += OnPointerEntered;
            newItem.PointerExited += OnPointerExited;
            newItem.LostFocus += OnLostFocus;
            newItem.RightTapped += OnRightTapped;
            newItem.PointerCaptureLost += OnPointerCaptureLost;

            // Assuming "CloseButton" is the name of the close button in your tab item
            var closeButton = newItem.FindName("CloseButton") as Button;

            if (closeButton != null)
            {
                closeButton.PointerEntered += CloseButton_PointerEntered;
                closeButton.PointerExited += OnCloseButtonPointerExited;
            }
        }

        private void OnPointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            // Show the flyout when the TabItem is clicked
            ShowFlyout((FireBrowserTabViewItem)sender);
        }
        private async void OnPointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            FireBrowserTabViewItem tabItem = (FireBrowserTabViewItem)sender;

            // Check if the pointer is over the close button
            if (IsPointerOverCloseButton(tabItem, e.GetCurrentPoint(tabItem).Position))
            {
                // Focus on the close button without showing the flyout
                FocusOnCloseButton(tabItem);
                // Reset the timer to avoid closing the flyout when over the close button
                ResetFlyoutCloseTimer();
            }
            else if (!isFlyoutOpen)
            {
                // Open the flyout when the pointer enters, if not already open
                await Task.Delay(50); // Introduce a slight delay before showing the flyout
                ShowFlyout(tabItem);
            }
        }

        private void CloseButton_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            // Focus on the close button when the pointer enters
            FocusOnCloseButton(FindTabItemFromCloseButton(sender));
        }

        private void OnPointerMoved(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            FireBrowserTabViewItem tabItem = (FireBrowserTabViewItem)sender;

            // Reset the timer if the flyout is open
            if (isFlyoutOpen && !IsPointerOverCloseButton(tabItem, e.GetCurrentPoint(tabItem).Position))
            {
                ResetFlyoutCloseTimer();
            }
        }

        private void OnPointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            FireBrowserTabViewItem tabItem = (FireBrowserTabViewItem)sender;

            if (isFlyoutOpen)
            {
                if (IsPointerOverCloseButton(tabItem, e.GetCurrentPoint(tabItem).Position))
                {
                    // If the pointer is over the close button, cancel the timer
                    ResetFlyoutCloseTimer();
                }
                else
                {
                    // Start the timer to close the flyout
                    StartFlyoutCloseTimer(tabItem);
                }
            }
        }

        private void OnCloseButtonPointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            // Remove focus when the pointer leaves the close button
            UnfocusOnCloseButton(FindTabItemFromCloseButton(sender));
        }

        private void UnfocusOnCloseButton(FireBrowserTabViewItem tabItem)
        {
            // Find the CloseButton in the visual tree of the tab item
            var closeButton = tabItem.FindName("CloseButton") as Button;

            if (closeButton != null)
            {
                closeButton.Focus(FocusState.Unfocused);
            }
        }

        private void OnPointerCaptureLost(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            FireBrowserTabViewItem tabItem = (FireBrowserTabViewItem)sender;

            // Check if the pointer capture is lost outside the tab item's parent container
            if (isFlyoutOpen && !IsPointerOverParentContainer(tabItem, e.GetCurrentPoint(tabItem.Parent as UIElement).Position))
            {
                // Start the timer to close the flyout
                StartFlyoutCloseTimer(tabItem);
            }
        }

        private bool IsPointerOverParentContainer(FireBrowserTabViewItem tabItem, Point pointerPosition)
        {
            // Replace this with the actual parent container type (e.g., Grid, Border)
            var parentContainer = tabItem.Parent as FrameworkElement;

            if (parentContainer != null)
            {
                GeneralTransform transform = parentContainer.TransformToVisual(tabItem);
                Point containerPosition = transform.TransformPoint(new Point(0, 0));

                Rect containerBounds = new Rect(containerPosition, new Size(parentContainer.ActualWidth, parentContainer.ActualHeight));

                // Check if the pointer is over the parent container
                return containerBounds.Contains(pointerPosition);
            }

            return false;
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            // Close the flyout only if it was opened by pointer
            if (isFlyoutOpen && sender is FireBrowserTabViewItem tabItem)
            {
                Point? pointerPosition = GetPointerPosition();

                if (pointerPosition != null)
                {
                    // Convert the pointer position to the tab item's coordinate system
                    GeneralTransform transform = tabItem.TransformToVisual(Window.Current.Content);
                    Point tabItemPosition = transform.TransformPoint(new Point(0, 0));
                    Rect tabBounds = new Rect(tabItemPosition, new Size(tabItem.ActualWidth, tabItem.ActualHeight));

                    // Check if the pointer is still over the tab item
                    if (tabBounds.Contains(pointerPosition.Value))
                    {
                        CloseFlyout(tabItem);
                    }
                }
            }
        }

        private void OnRightTapped(object sender, Microsoft.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            // Prevent the default context menu from appearing
            e.Handled = true;
        }

        private bool IsPointerOverCloseButton(FireBrowserTabViewItem tabItem, Point pointerPosition)
        {
            // Find the CloseButton in the visual tree of the tab item
            var closeButton = tabItem.FindName("CloseButton") as Button;

            if (closeButton != null)
            {
                GeneralTransform transform = closeButton.TransformToVisual(tabItem);
                Point buttonPosition = transform.TransformPoint(new Point(0, 0));

                Rect buttonBounds = new Rect(buttonPosition, new Size(closeButton.ActualWidth, closeButton.ActualHeight));

                // Check if the pointer is over the close button
                return buttonBounds.Contains(pointerPosition);
            }

            return false;
        }

        private void FocusOnCloseButton(FireBrowserTabViewItem tabItem)
        {
            // Find the CloseButton in the visual tree of the tab item
            var closeButton = tabItem.FindName("CloseButton") as Button;

            if (closeButton != null)
            {
                closeButton.Focus(FocusState.Programmatic);
            }
        }

        private void ShowFlyout(FireBrowserTabViewItem tabItem)
        {
            // Reactive the flyout only once
            if (!isFlyoutOpen)
            {
                tabsHover.ShowAt(tabItem);
                isFlyoutOpen = true;
                ResetFlyoutCloseTimer();
            }
        }

        private async void StartFlyoutCloseTimer(FireBrowserTabViewItem tabItem)
        {
            // Delay for 2.5 seconds before closing the flyout
            await Task.Delay(2500);

            // Close the flyout after the delay
            CloseFlyout(tabItem);

            // Delay for 3 seconds before allowing the flyout to open again
            await Task.Delay(3000);

            // Reset the timer to allow the flyout to open again
            ResetFlyoutCloseTimer();
        }

        private void ResetFlyoutCloseTimer()
        {
            // Reset the timer if it's running
            if (flyoutCloseTimer != null)
            {
                flyoutCloseTimer.Stop();
            }
        }

        private void CloseFlyout(FireBrowserTabViewItem tabItem)
        {
            if (tabsHover != null)
            {
                tabsHover.Hide();
                isFlyoutOpen = false;
                ResetFlyoutCloseTimer();
            }
        }

        // Helper method to get the pointer position in screen coordinates
        private Point? GetPointerPosition()
        {
            if (Window.Current != null && Window.Current.CoreWindow != null)
            {
                return Window.Current.CoreWindow.PointerPosition;
            }

            return null;
        }

        // Helper method to find the tab item associated with a close button
        private FireBrowserTabViewItem FindTabItemFromCloseButton(object closeButton)
        {
            // Find the visual tree upwards to locate the FireBrowserTabViewItem
            DependencyObject current = closeButton as DependencyObject;

            while (current != null)
            {
                if (current is FireBrowserTabViewItem tabItem)
                {
                    return tabItem;
                }

                current = VisualTreeHelper.GetParent(current);
            }

            return null;
        }
    }
}
