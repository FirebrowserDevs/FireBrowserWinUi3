using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using System.IO;
using Azure;
using Azure.Data.Tables;
using Windows.System;
using Windows.System.UserProfile;




public class UserEntity : ITableEntity
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public string Email { get; set; }
    public string BlobUrl { get; set; }

    public ETag ETag { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
}


namespace FireBrowserWinUi3.Services
{
    internal class AzBackupService : ObservableRecipient
    {
        private string AzureStorageConnectionString { get; set; }
        
        protected private void SET_AZConnectionsString(string connString)
        {
            Windows.Storage.ApplicationData.Current.LocalSettings.Values[nameof(AzureStorageConnectionString)] = AzureStorageConnectionString = connString;
        }

        protected internal string ConnString { get; } = Windows.Storage.ApplicationData.Current.LocalSettings.Values[nameof(AzureStorageConnectionString)] as string;
        protected internal string StoragAccountName { get; set; }
        protected internal string ContainerName { get; set; }
        protected internal User UserWindows { get; set; } 



        public AzBackupService( ) { 
        
        }
        public AzBackupService(string connString, string storagAccountName, string containerName, User user)
        {
            SET_AZConnectionsString(connString); 

        }

        public async Task<System.Collections.Generic.IReadOnlyList<User>> GetUserInformationAsync()
        {
            // Get the current user
            var users = await  User.FindAllAsync();

            // Assuming we're interested in the first user
            if (users.Count > 0)
            {
                var user = users[0];

                // Get the display name
                var displayName = await user.GetPropertyAsync(KnownUserProperties.DisplayName) as string;
                Console.WriteLine("Display Name: " + displayName);

                // Get the email address (Note: This might require enterprise policy permissions)
                var email = await user.GetPropertyAsync(KnownUserProperties.AccountName) as string;
                Console.WriteLine("Email: " + email);
            }
            return users;
        }

        public async Task InsertOrUpdateEntityAsync(string tableName, string email, string blobUrl)
        {
            var serviceClient = new TableServiceClient(ConnString);
            var tableClient = serviceClient.GetTableClient(tableName);

            // Create the table if it doesn't exist
            await tableClient.CreateIfNotExistsAsync();

            var entity = new UserEntity
            {
                PartitionKey = "Users",
                RowKey = Guid.NewGuid().ToString(),
                Email = email,
                BlobUrl = blobUrl
            };

            await tableClient.UpsertEntityAsync(entity);
        }
        private async void UploadAndStoreFile(string _blobName)
        {
            
            
            var tableName = "FireBatchBackups";
            var email = User.GetDefault().GetPropertyAsync(KnownUserProperties.AccountName).GetResults() as string; 
            var blobName = _blobName;

            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".png");
            var file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                var fileStream = await file.OpenReadAsync();
                await UploadFileToBlobAsync(ConnString, ContainerName,  blobName, fileStream);
                var blobUrl = $"https://your-storage-account-url.blob.core.windows.net/{ContainerName}/{blobName}";

                await InsertOrUpdateEntityAsync(tableName, email, blobUrl);
            }
        }

        private async Task UploadFileToBlobAsync(string connectionString, string containerName, string blobName, IRandomAccessStream fileStream)
        {
            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.UploadAsync(fileStream.AsStream(), true);
        }

    }



}
