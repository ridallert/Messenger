using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Network.Requests
{
    class LogOutRequest
    {
        public string Name { get; set; }

        public LogOutRequest(string name)
        {
            Name = name;
        }
        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(LogOutRequest),
                Payload = this
            };

            return container;
        }
    }
}
