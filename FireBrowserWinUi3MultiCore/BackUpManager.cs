using System;
using System.IO;
using System.IO.Compression;

namespace FireBrowserWinUi3MultiCore
{
    public static class BackupManager
    {
        public static string CreateBackup()
        {
            try
            {
                string currentDate = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string backupFileName = $"firebrowserbackup_{currentDate}.firebackup";
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string fireBrowserUserCorePath = Path.Combine(documentsPath, "FireBrowserUserCore");
                string backupFilePath = Path.Combine(documentsPath, backupFileName);

                if (!Directory.Exists(fireBrowserUserCorePath))
                {
                    throw new DirectoryNotFoundException($"FireBrowserUserCore folder not found in Documents: {fireBrowserUserCorePath}");
                }

                // Create zip file directly from the FireBrowserUserCore folder
                ZipFile.CreateFromDirectory(fireBrowserUserCorePath, backupFilePath);

                if (!File.Exists(backupFilePath))
                {
                    throw new FileNotFoundException("Backup file was not created successfully.");
                }

                return backupFilePath;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create backup: {ex.Message}", ex);
            }
        }
        public static bool RestoreBackup()
        {
            try
            {
                string restoreFilePath = Path.Combine(Path.GetTempPath(), "restore.fireback");
                if (!File.Exists(restoreFilePath))
                {
                    Console.WriteLine("Restore file does not exist.");
                    return false;
                }

                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string restorePath = Path.Combine(documentsPath, "FireBrowserUserCore");

                // Create a temporary directory for extraction
                string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempDir);

                try
                {
                    // Extract the backup to the temporary directory
                    ZipFile.ExtractToDirectory(restoreFilePath, tempDir);

                    // If FireBrowserUserCore already exists, rename it
                    if (Directory.Exists(restorePath))
                    {
                        string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                        string backupDir = $"{restorePath}_backup_{timestamp}";
                        Directory.Move(restorePath, backupDir);
                        Console.WriteLine($"Existing FireBrowserUserCore folder backed up to: {backupDir}");
                    }

                    // Move the extracted contents to FireBrowserUserCore
                    Directory.Move(tempDir, restorePath);

                    Console.WriteLine($"Backup restored successfully to: {restorePath}");
                    return true;
                }
                finally
                {
                    // Clean up temporary directory if it still exists
                    if (Directory.Exists(tempDir))
                    {
                        Directory.Delete(tempDir, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error restoring backup: {ex.Message}");
                return false;
            }
        }
    }
}