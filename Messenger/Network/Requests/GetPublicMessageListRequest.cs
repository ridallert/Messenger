using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Network.Requests
{
    public class GetPublicMessageListRequest
    {
        public string Name { get; set; }

        public GetPublicMessageListRequest(string name)
        {
            Name = name;
        }
        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(GetPublicMessageListRequest),
                Payload = this
            };

            return container;
        }
    }
}
