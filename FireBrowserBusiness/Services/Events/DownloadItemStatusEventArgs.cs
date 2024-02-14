﻿using Microsoft.UI.Xaml.Controls;
using System;

namespace FireBrowserBusiness.Services.Events
{
    public class DownloadItemStatusEventArgs : EventArgs
    {
        public enum EnumStatus
        {
            Added,
            Removed,
            Updated
        };
        public EnumStatus Status { get; set; }
        public string FilePath { get; set; }
        public ListViewItem DownloadedItem { get; set; }
    }
}
