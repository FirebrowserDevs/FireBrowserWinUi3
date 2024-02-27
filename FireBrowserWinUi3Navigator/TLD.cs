using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace FireBrowserWinUi3Navigator;

public class TLD
{
    public static string KnownDomains { get; set; }

    public static async Task LoadKnownDomainsAsync()
    {
        try
        {
            var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:////public_domains.txt"));
            KnownDomains = await new FireTxtReader().ReadTextFile(file);
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

    public static string GetTLDfromURL(string url) => url[(url.LastIndexOf(".") + 1)..];
}