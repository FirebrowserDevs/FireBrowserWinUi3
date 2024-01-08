using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;

namespace FireBrowserMultiCore.Helper;

public class ImageHelper : MarkupExtension
{
    private static readonly Dictionary<string, BitmapImage> ImageCache = new();

    public string ImageName { get; set; }

    protected override object ProvideValue() => LoadImage(ImageName);

    public BitmapImage LoadImage(string imageName)
    {
        if (string.IsNullOrEmpty(imageName)) return null;

        if (!ImageCache.TryGetValue(imageName, out var cachedImage))
        {
            var uri = new Uri($"ms-appx:///FireBrowserMultiCore//Assets/{imageName}");
            cachedImage = new BitmapImage(uri);
            ImageCache[imageName] = cachedImage;
        }

        return cachedImage;
    }
}
