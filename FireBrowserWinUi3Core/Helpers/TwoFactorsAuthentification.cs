using FireBrowserWinUi3Core.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace FireBrowserWinUi3Core.Helpers
{
    public static class TwoFactorsAuthentification
    {
        public static ObservableCollection<TwoFactorAuthItem> Items { get; private set; } = new();

        public static void Save()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(Items, options);

            File.WriteAllBytes(Data.TotpFilePath, EncryptionHelpers.ProtectString(jsonString));
        }

        public static async Task<ObservableCollection<TwoFactorAuthItem>> Load()
        {
            if (File.Exists(Data.TotpFilePath))
            {
                byte[] encryptedJsonString = File.ReadAllBytes(Data.TotpFilePath);
                string jsonString = EncryptionHelpers.UnprotectToString(encryptedJsonString);

                Items = !string.IsNullOrEmpty(jsonString)
                    ? JsonSerializer.Deserialize<ObservableCollection<TwoFactorAuthItem>>(jsonString)
                    : new ObservableCollection<TwoFactorAuthItem>();
            }

            Items.CollectionChanged += (_, _) => Save();
            await Task.CompletedTask;

            return Items;
        }

        public static async Task Repair()
        {
            try
            {
                // Check if 2fa.json file exists
                if (File.Exists(Data.TotpFilePath))
                {
                    byte[] encryptedJsonString = File.ReadAllBytes(Data.TotpFilePath);
                    string jsonString = EncryptionHelpers.UnprotectToString(encryptedJsonString);

                    if (string.IsNullOrEmpty(jsonString))
                    {
                        // If the file contents cannot be decrypted or are empty, recreate the file with default data
                        CreateDefaultFile();
                    }
                    else
                    {
                        // Try deserializing the file contents
                        var items = JsonSerializer.Deserialize<ObservableCollection<TwoFactorAuthItem>>(jsonString);
                        if (items == null)
                        {
                            // If deserialization fails, recreate the file with default data
                            CreateDefaultFile();
                        }
                        else
                        {
                            // If the file contents are valid, update the Items collection
                            Items = items;
                            Items.CollectionChanged += (_, _) => Save();
                        }
                    }
                }
                else
                {
                    // If the file does not exist, create it with default data
                    CreateDefaultFile();
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the repair process
                Console.WriteLine($"Error repairing 2fa.json file: {ex.Message}");
            }

            await Task.CompletedTask;
        }

        private static void CreateDefaultFile()
        {
            Items = new ObservableCollection<TwoFactorAuthItem>();
            Items.CollectionChanged += (_, _) => Save();
            Save();
        }
    }
}