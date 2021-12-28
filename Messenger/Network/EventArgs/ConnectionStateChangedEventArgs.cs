using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Network.EventArgs
{
    public class ConnectionStateChangedEventArgs
    {
        public string ClientName { get; }
        public bool Connected { get; }
        public ConnectionStateChangedEventArgs(string clientName, bool connected)
        {
            ClientName = clientName;
            Connected = connected;
        }
    }
}
