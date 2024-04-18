using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireBrowserWinUi3Services.PluginCore
{
    public class PluginEntry
    {
        public String Name { get; set; }
        public String Description { get; set; }

        IPluginBase Plugin = null;

        public UserControl form { get; set; } = null;

        public PluginEntry(IPluginBase p)
        {
            Plugin = p;

            this.Name = p.Name;
            this.Description = p.Description;

            Plugin.Initialize(null);
            //form = (UserControl)Plugin.DynamicValues[1];  dynamiccontrols dont work yet
        }
    }
}
