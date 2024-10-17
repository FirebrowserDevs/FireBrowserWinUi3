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
using System.Linq;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using System.Collections.Generic;
using FireBrowserWinUi3Exceptions;



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
    internal class AzBackupService 
    {
        private string AzureStorageConnectionString { get; set; }
        
        protected private void SET_AZConnectionsString(string connString)
        {
            Windows.Storage.ApplicationData.Current.LocalSettings.Values[nameof(AzureStorageConnectionString)] = AzureStorageConnectionString = connString;
        }

        protected internal string ConnString { get; } = Windows.Storage.ApplicationData.Current.LocalSettings.Values[nameof(AzureStorageConnectionString)] as string;
        protected internal string StoragAccountName { get; set; }
        protected internal string ContainerName { get; set; }
        protected internal object UserWindows { get; set; } 

        protected FireBrowserWinUi3MultiCore.User FireUser { get;set; }


        private AzBackupService(FireBrowserWinUi3MultiCore.User fireUser, string _storageName, string _containerName) {

            StoragAccountName = _storageName; 
            ContainerName = _containerName ?? string.Empty;
            FireUser = fireUser;   

        }
        public AzBackupService(string connString, string storagAccountName, string containerName, FireBrowserWinUi3MultiCore.User user) : this(user, storagAccountName, containerName)
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
                await UploadFileToBlobAsync(blobName, fileStream);
                var blobUrl = $"https://your-storage-account-url.blob.core.windows.net/{ContainerName}/{blobName}";

                await InsertOrUpdateEntityAsync(tableName, email, blobUrl);
            }
        }
              
        public async Task<object> UploadFileToBlobAsync(string blobName, IRandomAccessStream fileStream)
        {
            try
            {
                var storageAccount = CloudStorageAccount.Parse(ConnString);
                var blobClient = storageAccount.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference("firebackups");
                var response = new object();
                var fileName = Guid.NewGuid().ToString() + "_" + blobName;
                // Upload the file to Azure Blob Storage
                var blockBlob = container.GetBlockBlobReference(fileName);

                await blockBlob.UploadFromStreamAsync(fileStream.AsStream());
                var blob = container.GetBlockBlobReference(fileName);

                var sasToken = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy
                {
                    Permissions = SharedAccessBlobPermissions.Read,
                    SharedAccessExpiryTime = DateTime.UtcNow.AddHours(1) // Token valid for 1 hour
                });

                var sasUrl = blockBlob.Uri.AbsoluteUri + sasToken; //blockBlob.Uri.AbsoluteUri

                return response = new
                {
                    FileName = blobName,
                    Url = sasUrl.ToString()
                };
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                return Task.FromException(ex);  
            }
                
        }
    }
}
