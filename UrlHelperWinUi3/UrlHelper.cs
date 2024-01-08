using System;
using System.Linq;

namespace UrlHelperWinUi3;
public class UrlHelper
{
    public static string GetInputType(string input)
    {
        return IsURL(input) ? "url" : IsURLWithoutProtocol(input) ? "urlNOProtocol" : "searchquery";
    }

    private static bool IsURL(string input)
    {
        return Uri.TryCreate(input, UriKind.Absolute, out Uri uri) &&
               (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }

    private static bool IsURLWithoutProtocol(string input)
    {
        string tld = TLD.GetTLDfromURL(input);
        return input.Contains(".") && TLD.KnownDomains?.Any(domain => tld.Contains(domain)) == true;
    }
}