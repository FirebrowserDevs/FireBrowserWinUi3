﻿using Microsoft.UI.Xaml.Markup;
using Microsoft.Windows.ApplicationModel.Resources;
using System.Text;

namespace FireBrowserWinUi3.Controls
{
    [MarkupExtensionReturnType(ReturnType = typeof(string))]
    public sealed class ResourceString : MarkupExtension
    {
        private static readonly ResourceLoader resourceLoader = new();

        public string Name { get; set; } = string.Empty;

        protected override object ProvideValue() => resourceLoader.GetString(Name);
    }
}
