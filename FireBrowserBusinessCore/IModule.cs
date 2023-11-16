using Microsoft.UI.Xaml;
using System.Threading.Tasks;

namespace FireBrowserBusinessCore
{
    public interface IModule
    {
        Task<UIElement> GetModuleUIAsync();
    }

}
