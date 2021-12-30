using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Network.Requests
{
    public class GetMessageListRequest
    {
        public string Name { get; set; }

        public GetMessageListRequest(string name)
        {
            Name = name;
        }

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(GetMessageListRequest),
                Payload = this
            };

            return container;
        }
    }
}
