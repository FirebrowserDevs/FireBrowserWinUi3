using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;


namespace UrlHelperWinUi3;

public class TLD
{
    public static string KnownDomains { get; set; }

    public static async Task LoadKnownDomainsAsync()
    {
        try
        {
            var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(
                 new Uri($"ms-appx:////public_domains.txt"));
            KnownDomains = await FileIO.ReadTextAsync(file);
        }
        catch (FileNotFoundException ex)
        {
            // Handle the case where the file is not found
            Debug.WriteLine("File not found: " + ex.Message);
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            Debug.WriteLine("An error occurred: " + ex.Message);
        }
    }

    public static string GetTLDfromURL(string url)
    {
        int pos = url.LastIndexOf(".") + 1;
        string tld = url.Substring(pos, url.Length - pos);
        return tld;
    }
}
