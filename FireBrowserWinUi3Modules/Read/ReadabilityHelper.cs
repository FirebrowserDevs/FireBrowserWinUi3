using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage;

namespace FireBrowserWinUiModules.Read
{
    public class ReadabilityHelper
    {
        public static async Task<string> GetReadabilityScriptAsync()
        {
            try
            {
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///FireBrowserWinUiModules/Read/Jscript/readability.js"));
                string jscript = await FileIO.ReadTextAsync(file);
                return jscript;
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately (e.g., log, throw, or return a default script)
                Debug.WriteLine($"Error loading ReadScript: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
