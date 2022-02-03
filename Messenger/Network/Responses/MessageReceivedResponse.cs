using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Network.Responses
{
    public class MessageReceivedResponse
    {
        public int MessageId { get; set; }
        public int SenderId { get; set; }
        public int ChatId { get; set; }
        public string SenderName { get; set; }
        public string Text { get; set; }
        public DateTime SendTime { get; set; }

        public MessageReceivedResponse(int messageId, int senderId, int chatId, string senderName, string text, DateTime sendTime)
        {
            MessageId = messageId;
            SenderId = senderId;
            ChatId = chatId;
            SenderName = senderName;
            Text = text;
            SendTime = sendTime;
        }

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(MessageReceivedResponse),
                Payload = this
            };

            return container;
        }
    }
}
