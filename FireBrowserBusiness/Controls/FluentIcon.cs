using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;

namespace FireBrowserBusiness.Controls
{
    public sealed partial class FluentIcon : CommunityToolkit.WinUI.UI.FontIconExtension
    {
        private static readonly Dictionary<string, FontFamily> FontFamilyCache = new Dictionary<string, FontFamily>();

        public FluentIcon()
        {
            string fontFamilyKey = "FluentIcons";

            if (!FontFamilyCache.ContainsKey(fontFamilyKey))
            {
                if (Application.Current.Resources.TryGetValue(fontFamilyKey, out object fontFamilyObj) && fontFamilyObj is FontFamily fontFamily)
                {
                    FontFamilyCache[fontFamilyKey] = fontFamily;
                }
                else
                {
                    // Handle the case where the FluentIcons font family is not found in resources
                    throw new InvalidOperationException("FluentIcons font family not found in resources.");
                }
            }

            FontFamily = FontFamilyCache[fontFamilyKey];
        }
    }
}