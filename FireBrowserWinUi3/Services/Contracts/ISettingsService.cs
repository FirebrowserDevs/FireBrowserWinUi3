using FireBrowserWinUi3DataCore.Actions;
using System.Threading.Tasks;

namespace FireBrowserWinUi3.Services.Contracts
{
    public interface ISettingsService
    {
        //ObservableRecipient ViewModel { get; set; }
        Task SaveChangesToSettings(FireBrowserWinUi3MultiCore.User user, FireBrowserWinUi3MultiCore.Settings settings);
        FireBrowserWinUi3MultiCore.Settings CoreSettings { get; set; }

        SettingsActions Actions { get; set; }

    }
}
