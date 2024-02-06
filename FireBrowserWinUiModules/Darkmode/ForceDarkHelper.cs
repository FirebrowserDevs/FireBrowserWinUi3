using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace FireBrowserWinUiModules.Darkmode
{
    public class ForceDarkHelper
    {
        public static async Task<string> GetForceDarkScriptAsync()
        {
            try
            {
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///FireBrowserWinUiModules/DarkMode/Jscript/darkmode.js"));
                string jscript = await FileIO.ReadTextAsync(file);
                return jscript;
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately (e.g., log, throw, or return a default script)
                Debug.WriteLine($"Error loading ForceDarkScript: {ex.Message}");
                return string.Empty;
            }
        }

    }
}
