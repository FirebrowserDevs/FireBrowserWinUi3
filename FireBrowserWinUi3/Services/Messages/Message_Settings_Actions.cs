using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireBrowserWinUi3.Services.Messages
{
    public enum EnumMessageStatus
    {
        Added,
        Removed,
        Updated
    };
    public record class Message_Settings_Actions(string payload, EnumMessageStatus Status)
    {
        public Message_Settings_Actions(string payload): this(payload, EnumMessageStatus.Updated) {

            Payload = payload; 
        }

        public string Payload { get; }
    }
}
