﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FireBrowserWinUi3QrCore.PayloadGenerator;

namespace FireBrowserWinUi3.Services.Messages
{
    public enum EnumMessageStatus
    {
        Added,
        Login,
        Logout,
        Removed,
        Settings,
        Updated
    };
    public record class Message_Settings_Actions(string _payload, EnumMessageStatus _status)
    {
        public Message_Settings_Actions(string payload): this(payload, EnumMessageStatus.Updated) {

            Payload = payload;
            Status = this._status;
        }

        public Message_Settings_Actions(EnumMessageStatus _status) : this(null, _status) {

            Payload = this._payload;    
            Status = _status;
        }

        public EnumMessageStatus Status { get; } = _status;
        public string Payload { get; } = _payload; 
    }
}