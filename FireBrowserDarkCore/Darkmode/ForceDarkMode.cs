using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace FireBrowserDarkCore.Darkmode
{
    public class ForceDarkMode
    {
        public static async Task<string> GetForceDark()
        {

            try
            {
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///FireBrowserDarkCore/Darkmode/Jscript/forcedark.js"));
                string jscript = await FileIO.ReadTextAsync(file);
                return jscript;
            }
            catch (FileNotFoundException)
            {
                // Handle the case where the file is not found
                return "File not found.";
            }


        }
    }
}
