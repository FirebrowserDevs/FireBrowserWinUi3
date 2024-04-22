﻿using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireBrowserWinUi3Services.PluginCore
{
    public class XamlPluginEntry : PluginEntry
    {
        public XamlPluginEntry(IPluginBase p) : base(p)
        {
            // Check if the plugin response contains a XAML form
            if (p is IPluginCore.RpResponse xamlResponse && xamlResponse.Form is UserControl xamlForm)
            {
                // Assign the XAML form to the base class's form property
                form = xamlForm;
            }
        }
    }
}
