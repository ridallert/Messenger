using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Network.Requests
{
    class GetPrivateMessageListRequest
    {
        public string Sender { get; set; }
        public string Receiver { get; set; }

        public GetPrivateMessageListRequest(string sender, string receiver)
        {
            Sender = sender;
            Receiver = receiver;
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
