using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.Email;
using Windows.Storage.Streams;

namespace FireBrowserWinUi3.Services.ViewModels
{
    public partial class UploadBackupViewModel : ObservableRecipient
    {
        public ObservableCollection<EmailUser> Users { get; set; }
        public EmailUser SelectedUser { get; set; }

        public UploadBackupViewModel()
        {
            Users = new ObservableCollection<EmailUser>
            {
                new EmailUser { Name = "User1", Email = "user1@example.com" },
                new EmailUser { Name = "User2", Email = "user2@example.com" }
            };

        }

        [RelayCommand]
        private void SelectFile()
        {
            // Logic to select file
        }
        private async Task SendEmailAsync(string toEmail, string sasUrl)
        {
            var email = new EmailMessage();
            email.Subject = "Bug Report";
            email.To.Add(new EmailRecipient("firebrowserdevs@gmail.com"));
            email.Body = "Any Extra Info Can Help Find The Bug.\nPlease find attached file for the bug report above.";
            var fileStream = await file.OpenReadAsync();
            var contentType = file.ContentType;
            var streamRef = RandomAccessStreamReference.CreateFromStream(fileStream);
            email.Attachments.Add(new EmailAttachment(file.Name, streamRef, contentType));
            await EmailManager.ShowComposeNewEmailAsync(email);
        }
        
        [RelayCommand]
        private void GenerateAndSend()
        {
            // Example SAS generation logic
            string blobUri = "https://yourstorageaccount.blob.core.windows.net/yourcontainer/yourfile";
            BlobServiceClient blobServiceClient = new BlobServiceClient("YourConnectionString");
            BlobClient blobClient = new BlobClient(new Uri(blobUri));

            BlobSasBuilder sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = blobClient.BlobContainerName,
                BlobName = blobClient.Name,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5),
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
            };
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            Uri sasUri = blobClient.GenerateSasUri(sasBuilder);

            // Logic to send email
            SendEmailAsync(SelectedUser.Email, sasUri.ToString());
        }

        
    }
         
     
        public class EmailUser
        {
            public string Name { get; set; }
            public string Email { get; set; }
        }
    
    
}
