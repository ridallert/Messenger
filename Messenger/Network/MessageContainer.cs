using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Network
{
    class MessageContainer
    {
        public string Identifier { get; set; }
        public object Payload { get; set; }
    }
}
