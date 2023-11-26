using Microsoft.UI.Xaml.Markup;
using Microsoft.Windows.ApplicationModel.Resources;

namespace FireBrowserWinUi3.Controls;

[MarkupExtensionReturnType(ReturnType = typeof(string))]
public sealed class ResourceString : MarkupExtension
{
    private static ResourceLoader resourceLoader;

    public string Name { get; set; } = string.Empty;
    public string Filename { get; set; } = string.Empty;

    public static string GetString(string name, string filename)
    {
        resourceLoader = new(filename);
        return resourceLoader.GetString(name);
    }

    protected override object ProvideValue()
    {
        resourceLoader = new(Filename);
        return resourceLoader.GetString(Name);

    }
}