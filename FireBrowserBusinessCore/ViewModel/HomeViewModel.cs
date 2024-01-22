using CommunityToolkit.Mvvm.ComponentModel;
using FireBrowserBusinessCore.Models;

namespace FireBrowserCore.ViewModel
{
    public partial class HomeViewModel : ObservableObject
    {
        private Settings.NewTabBackground _backgroundType;
        private string _imageTitle;
        private string _imageCopyright;
        private string _imageCopyrightLink;

        public Settings.NewTabBackground BackgroundType
        {
            get => _backgroundType;
            set => SetProperty(ref _backgroundType, value);
        }

        public string ImageTitle
        {
            get => _imageTitle;
            set => SetProperty(ref _imageTitle, value);
        }

        public string ImageCopyright
        {
            get => _imageCopyright;
            set => SetProperty(ref _imageCopyright, value);
        }

        public string ImageCopyrightLink
        {
            get => _imageCopyrightLink;
            set => SetProperty(ref _imageCopyrightLink, value);
        }
    }
}