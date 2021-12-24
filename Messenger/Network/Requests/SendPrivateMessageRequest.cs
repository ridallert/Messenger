using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Network.Requests
{
    class SendPrivateMessageRequest
    {
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Text { get; set; }
        public DateTime SendTime { get; set; }

        public SendPrivateMessageRequest(string sender, string receiver, string text, DateTime sendTime)
        {
            Sender = sender;
            Receiver = receiver;
            Text = text;
            SendTime = sendTime;
        }

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(SendPrivateMessageRequest),
                Payload = this
            };

            return container;
        }
    }
}
