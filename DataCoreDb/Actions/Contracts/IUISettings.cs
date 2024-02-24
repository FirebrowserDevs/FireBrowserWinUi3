using System.Threading.Tasks;

namespace FireBrowserDataCore.Actions.Contracts
{
    public interface IUISettings
    {
        Task<bool> UpdateSettingsAsync(FireBrowserMultiCore.Settings settings);
        Task<FireBrowserMultiCore.Settings> GetSettingsAsync();
        SettingsContext SettingsContext { get; }

    }
}
