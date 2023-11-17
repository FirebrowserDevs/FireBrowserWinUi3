using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Threading.Tasks;

namespace FireBrowserBusinessCore
{
    public interface IModule
    {
        public async Task<Page> GetModulePageAsync()
        {
            return await Task.FromResult(new Page()); // Placeholder return statement
        }
    }

}
