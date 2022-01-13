using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Network.Responses
{
    public class PublicMessageReceivedResponse
    {
        public string Sender { get; set; }
        public string Text { get; set; }
        public DateTime SendTime { get; set; }

        public PublicMessageReceivedResponse(string sender, string text, DateTime sendTime)
        {
            Sender = sender;
            Text = text;
            SendTime = sendTime;
        }

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(PublicMessageReceivedResponse),
                Payload = this
            };

            return container;
        }
    }
}
