using System;

namespace Messenger.Network.Responses
{
    public class PrivateMessageReceivedResponse
    {
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Text { get; set; }
        public DateTime SendTime { get; set; }

        public PrivateMessageReceivedResponse(string sender, string receiver, string text, DateTime sendTime)
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
                Identifier = nameof(PrivateMessageReceivedResponse),
                Payload = this
            };

            return container;
        }
    }
}
