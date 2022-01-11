using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Network.Requests
{
    class GetContactsRequest
    {
        public string Name { get; set; }

        public GetContactsRequest(string name)
        {
            Name = name;
        }
        public MessageContainer GetContainer()
        {

            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(GetContactsRequest),
                Payload = this
            };

            return container;
        }
    }
}
