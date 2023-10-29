using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;

namespace Assets;

public class ImageLoader : MarkupExtension
{
    private static readonly Dictionary<string, BitmapImage> ImageCache = new();

    public string ImageName { get; set; }

    protected override object ProvideValue()
    {
        if (string.IsNullOrEmpty(ImageName))
        {
            return null;
        }

        if (ImageCache.TryGetValue(ImageName, out var cachedImage))
        {
            return cachedImage;
        }

        var uri = new Uri($"ms-appx:///Assets//Assets/{ImageName}");
        var bitmapImage = new BitmapImage(uri);
        ImageCache[ImageName] = bitmapImage;

        return bitmapImage;
    }
}