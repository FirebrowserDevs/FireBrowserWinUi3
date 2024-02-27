using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace FireBrowserWinUi3.Services.Contracts;
public interface IServiceDownloads
{
    ObservableCollection<FireBrowserWinUi3.Controls.DownloadItem> DownloadItemControls { get; }
    Task SaveAsync(string FileName);
    Task<bool> DeleteAsync(string FilePath);
    Task UpdateAsync();
    Task InsertAsync(string current_path, string end_time, long start_time);
}