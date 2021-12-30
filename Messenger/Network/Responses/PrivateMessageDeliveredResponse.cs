using System;

namespace Messenger.Network.Responses
{
    class PrivateMessageDeliveredResponse
    {
        public string Sender { get; set; }
        public string Text { get; set; }
        public DateTime SendTime { get; set; }

        public PrivateMessageDeliveredResponse(string sender, string text, DateTime sendTime)
        {
            Sender = sender;
            Text = text;
            SendTime = sendTime;
        }

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(PrivateMessageDeliveredResponse),
                Payload = this
            };

            return container;
        }
    }
}
