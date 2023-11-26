using System;
using System.Linq;

namespace UrlHelperWinUi3;
public class UrlHelper
{
    public static string GetInputType(string input)
    {
        if (IsURL(input))
        {
            return "url";
        }
        else if (IsURLWithoutProtocol(input))
        {
            return "urlNOProtocol";
        }

        return "searchquery";
    }

    private static bool IsURL(string input)
    {
        if (Uri.TryCreate(input, UriKind.Absolute, out Uri uri) &&
            (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
        {
            return true;
        }

        return false;
    }

    private static bool IsURLWithoutProtocol(string input)
    {
        string tld = TLD.GetTLDfromURL(input);
        return input.Contains(".") && TLD.KnownDomains.Any(domain => tld.Contains(domain));
    }
}