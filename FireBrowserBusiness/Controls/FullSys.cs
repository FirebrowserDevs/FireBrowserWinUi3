using System;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.UI.ViewManagement;

namespace FireBrowserWinUi3.Controls
{
    public class FullSys
    {
        #region fullscreensys
        private bool fullScreen = false;

        [DefaultValue(false)]
        public bool FullScreen
        {
            get { return fullScreen; }
            set
            {
                ApplicationView view = ApplicationView.GetForCurrentView();
                if (value)
                {
                    try
                    {
                        if (!view.IsFullScreenMode)
                        {
                            view.TryEnterFullScreenMode();
                            Controls.UseContent.MainPageContent.HideToolbar(true);
                        }
                    }
                    catch (Exception ex)
                    {
                        
                    }
                }
                else
                {
                    try
                    {
                        if (view.IsFullScreenMode)
                        {
                            view.ExitFullScreenMode();
                            Controls.UseContent.MainPageContent.HideToolbar(false);
                        }
                    }
                    catch (Exception ex)
                    {
                       
                    }
                }
                fullScreen = value;
            }
        }

        #endregion
    }
}
