using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;

namespace Assets
{
    public class ImageLoader : MarkupExtension
    {
        private static readonly Dictionary<string, BitmapImage> ImageCache = new();

        public string ImageName { get; set; }

        protected override object ProvideValue()
        {
            return LoadImage(ImageName);
        }

        public BitmapImage LoadImage(string imageName)
        {
            if (string.IsNullOrEmpty(imageName))
            {
                return null;
            }

            if (ImageCache.TryGetValue(imageName, out var cachedImage))
            {
                return cachedImage;
            }

            // Initialize cachedImage to avoid the "unassigned local variable" error
            cachedImage = null;

            var uri = new Uri($"ms-appx:///Assets/Assets/{imageName}");
            cachedImage = new BitmapImage(uri);
            ImageCache[imageName] = cachedImage;

            return cachedImage;
        }
    }
}