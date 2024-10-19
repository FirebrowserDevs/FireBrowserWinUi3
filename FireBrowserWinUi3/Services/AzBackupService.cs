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
using static FireBrowserWinUi3Services.PluginCore.IPluginCore;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using System.Text;
using FireBrowserWinUi3MultiCore;



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

        [DllImport("secur32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetUserNameEx(int nameFormat, StringBuilder userName, ref uint userNameSize);

        public class UserNames
        {
            public string Unknown { get; set; }
            public string FullyQualifiedDN { get; set; }
            public string SamCompatible { get; set; }
            public string Display { get; set; }
            public string UniqueId { get; set; }
            public string Canonical { get; set; }
            public string UserPrincipal { get; set; }
            public string CanonicalEx { get; set; }
            public string ServicePrincipal { get; set; }
            public string DnsDomain { get; set; }
        }
        public static class NameFormats
        {
            public const int NameUnknown = 0;
            public const int NameFullyQualifiedDN = 1;
            public const int NameSamCompatible = 2;
            public const int NameDisplay = 3;
            public const int NameUniqueId = 6;
            public const int NameCanonical = 7;
            public const int NameUserPrincipal = 8;
            public const int NameCanonicalEx = 9;
            public const int NameServicePrincipal = 10;
            public const int NameDnsDomain = 12;
        }
        public static UserNames GetAllUserNames()
        {
            var userNames = new UserNames();
            uint size = 1024;
            StringBuilder name = new StringBuilder((int)size);

            userNames.Unknown = GetUserName(NameFormats.NameUnknown, name, ref size);
            userNames.FullyQualifiedDN = GetUserName(NameFormats.NameFullyQualifiedDN, name, ref size);
            userNames.SamCompatible = GetUserName(NameFormats.NameSamCompatible, name, ref size);
            userNames.Display = GetUserName(NameFormats.NameDisplay, name, ref size);
            userNames.UniqueId = GetUserName(NameFormats.NameUniqueId, name, ref size);
            userNames.Canonical = GetUserName(NameFormats.NameCanonical, name, ref size);
            userNames.UserPrincipal = GetUserName(NameFormats.NameUserPrincipal, name, ref size);
            userNames.CanonicalEx = GetUserName(NameFormats.NameCanonicalEx, name, ref size);
            userNames.ServicePrincipal = GetUserName(NameFormats.NameServicePrincipal, name, ref size);
            userNames.DnsDomain = GetUserName(NameFormats.NameDnsDomain, name, ref size);

            return userNames;
        }

        private static string GetUserName(int nameFormat, StringBuilder name, ref uint size)
        {
            name.Clear();
            size = 1024;
            if (GetUserNameEx(nameFormat, name, ref size))
            {
                return name.ToString();
            }
            else
            {
                return $"Error: {Marshal.GetLastWin32Error()}";
            }
        }
        public  Task<FireBrowserWinUi3MultiCore.User> GetUserInformationAsync()
        {
            uint size = 1024;
            StringBuilder name = new StringBuilder((int)size);
            var user = AuthService.CurrentUser ?? new FireBrowserWinUi3MultiCore.User();

            try
            {
                if (GetUserNameEx(NameFormats.NameDisplay, name, ref size))
                {
                    user.WindowsUserName = name.ToString();
                }
                if (GetUserNameEx(NameFormats.NameUserPrincipal, name, ref size))
                {
                    user.Email = name.ToString();
                }
                return Task.FromResult(user); 
            }
            catch (Exception)
            {

                throw;
            }
            
            // Get the current user
            //var users = await  User.FindAllAsync(UserType.SystemManaged);
            //User user = default; 
            //// Assuming we're interested in the first user
            //if (users.Count > 0)
            //{
            //    user = users[0];

            //    // Get the display name
            //    var displayName = await user.GetPropertyAsync(KnownUserProperties.DisplayName) as string;
            //    Console.WriteLine("Display Name: " + displayName);

            //    // Get the email address (Note: This might require enterprise policy permissions)
            //    var email = await user.GetPropertyAsync(KnownUserProperties.AccountName) as string;
            //    Console.WriteLine("Email: " + email);

                
            //}

            //return user; 

        }

        public class ResponseAZFILE(string blobName, object sasUrl)
        {
            public string FileName => blobName;
            public object Url => sasUrl.ToString();

            public override string ToString()
            {
                return JsonConvert.SerializeObject(this); 
            }
        }
        /*
         *       response = new
                {
                    FileName = blobName,
                    Url = sasUrl.ToString()
                };
         */
        public async Task InsertOrUpdateEntityAsync(string tableName, string email, string blobUrl)
        {
            try
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
            catch (Exception)
            {
                throw;
            }
            
        }
        public async Task<ResponseAZFILE> UploadAndStoreFile(string blobName, IRandomAccessStream fileStream)
        {
            try
            {
                var email = await GetUserInformationAsync();
                //var email = await user.GetPropertyAsync(KnownUserProperties.AccountName) as string;

                var result = await UploadFileToBlobAsync(blobName, fileStream);
                
                if(result is not null)
                    await InsertOrUpdateEntityAsync("TrackerBackups", email.WindowsUserName, result.Url.ToString());

                return result; 
            }
            catch (Exception ex)
            {
                return new ResponseAZFILE(ex.StackTrace, ex.Message!);    
             
            }
            
            
         
        }
              
        public async Task<ResponseAZFILE> UploadFileToBlobAsync(string blobName, IRandomAccessStream fileStream)
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

                return new ResponseAZFILE(blobName, sasUrl);

            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
                throw; 
            }
                
        }
    }
}
