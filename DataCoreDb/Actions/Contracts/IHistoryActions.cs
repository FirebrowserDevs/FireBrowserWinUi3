using System.Threading.Tasks;

namespace FireBrowserDataCore.Actions.Contracts
{
    public interface IHistoryActions
    {
        Task InsertHistoryItem(string url, string title, int visitCount, int typedCount, int hidden);
        Task DeleteHistoryItem(string url);
        HistoryContext HistoryContext { get; }

    }
}
