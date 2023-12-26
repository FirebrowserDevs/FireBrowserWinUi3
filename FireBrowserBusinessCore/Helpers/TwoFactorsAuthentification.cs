using FireBrowserBusinessCore.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace FireBrowserBusinessCore.Helpers
{
    public static class TwoFactorsAuthentification
    {
        public static ObservableCollection<TwoFactorAuthItem> Items { get; private set; }

        public static void Save()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(Items, options);

            byte[] encryptedJsonString = EncryptionHelpers.ProtectString(jsonString);
            File.WriteAllBytes(Data.TotpFilePath, encryptedJsonString);
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
            else
            {
                Items = new ObservableCollection<TwoFactorAuthItem>();
            }

            Items.CollectionChanged += (_, _) => Save();
            await Task.CompletedTask;

            return Items;
        }
    }
}