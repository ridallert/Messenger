using Messenger.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Network.Responses
{
    public class NewChatCreatedResponse
    {
        public Chat Chat { get; set; }

        public NewChatCreatedResponse(Chat chat)
        {
            Chat = chat;
        }

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(NewChatCreatedResponse),
                Payload = this
            };

            return container;
        }
    }
}
