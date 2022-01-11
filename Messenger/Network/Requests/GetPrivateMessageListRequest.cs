using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Network.Requests
{
    class GetPrivateMessageListRequest
    {
        public string Name { get; set; }

        public GetPrivateMessageListRequest(string name)
        {
            Name = name;
        }

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(GetPrivateMessageListRequest),
                Payload = this
            };

            return container;
        }
    }
}
