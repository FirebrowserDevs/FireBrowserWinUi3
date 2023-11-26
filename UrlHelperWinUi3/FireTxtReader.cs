using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace UrlHelperWinUi3;
public class FireTxtReader
{
    private const int BufferSize = (int)(1.5 * 1024 * 1024); // 1.5 MB buffer size

    public async Task<string> ReadTextFile(StorageFile file)
    {
        try
        {
            using (Stream stream = await file.OpenStreamForReadAsync())
            {
                StringBuilder stringBuilder = new StringBuilder();
                byte[] buffer = new byte[BufferSize];
                int bytesRead;

                while ((bytesRead = await stream.ReadAsync(buffer, 0, BufferSize)) > 0)
                {
                    string chunk = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    stringBuilder.Append(chunk);
                }

                return stringBuilder.ToString();
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions (e.g., file not found, permissions, etc.)
            Console.WriteLine($"Error reading file: {ex.Message}");
            return string.Empty;
        }
    }
}