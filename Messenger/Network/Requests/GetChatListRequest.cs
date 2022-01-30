using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Network.Requests
{
    public class GetChatListRequest
    {
        public string Name { get; set; }

        public GetChatListRequest(string name)
        {
            Name = name;
        }
        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(GetChatListRequest),
                Payload = this
            };

            return container;
        }
    }
}
