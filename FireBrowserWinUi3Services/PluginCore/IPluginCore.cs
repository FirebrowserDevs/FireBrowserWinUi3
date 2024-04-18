using System;
using System.Collections.Generic;
using System.Data.Common;

namespace FireBrowserWinUi3Services.PluginCore
{
    public class IPluginCore
    {
        public class PluginParameters
        {
            public DbConnection con { get; set; } = null;
            public HashSet<object> Context { get; set; } = new HashSet<object>();
        }

        public class PluginResponse
        {
            public String Message { get; set; } = "";
            public Boolean HasError { get; set; } = false;
            public String MessageID { get; set; } = "";
        }
    }
}
