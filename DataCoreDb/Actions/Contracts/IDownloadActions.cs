using System.Threading.Tasks;

namespace FireBrowserDataCore.Actions.Contracts
{
    public interface IDownloadActions
    {
        Task InsertDownloadItem(string guid, string current_path, string end_time, long start_time);
        Task DeleteDownloadItem(string current_path);
        DownloadContext DownloadContext { get; }
    }
}
