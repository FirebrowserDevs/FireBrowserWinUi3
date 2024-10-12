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

        public static string ReadBackupFile()
        {
            try
            {
                // Set the backup file path (restore.fireback in the temp directory)
                string restoreFilePath = Path.Combine(Path.GetTempPath(), "restore.fireback");

                // Check if the file exists
                if (!File.Exists(restoreFilePath))
                {
                    Console.WriteLine("Restore file does not exist.");
                    return null; // Return null if the file does not exist
                }

                // Read the file's contents and return it as a string
                string fileContents = File.ReadAllText(restoreFilePath);
                Console.WriteLine("File read successfully.");
                return fileContents;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading backup file: {ex.Message}");
                return null; // Return null if an error occurs
            }
        }

        public static bool RestoreBackup()
        {
            try
            {
                // Set the backup file path
                string restoreFilePath = Path.Combine(Path.GetTempPath(), "restore.fireback");
                string restorefile = ReadBackupFile();
                if (!File.Exists(restoreFilePath))
                {
                    Console.WriteLine("Restore file does not exist.");
                    return false;
                }

                // Set the target restore directory (FireBrowserUserCore)
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string restorePath = Path.Combine(documentsPath, "FireBrowserUserCore");

                // If FireBrowserUserCore exists, delete it
                if (Directory.Exists(restorePath))
                {
                    Directory.Delete(restorePath, true);
                    Console.WriteLine("Existing FireBrowserUserCore folder deleted.");
                }

                // Create the FireBrowserUserCore folder
                Directory.CreateDirectory(restorePath);

                // Extract the backup file to FireBrowserUserCore
                ZipFile.ExtractToDirectory(restorefile, restorePath);

                Console.WriteLine($"Backup restored successfully to: {restorePath}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error restoring backup: {ex.Message}");
                return false;
            }
        }

    }
}