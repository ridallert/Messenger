using Messenger.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Network.Responses
{
    public class GetChatListResponse
    {
        public string Result { get; set; }
        public List<Chat> ChatList { get; set; }

        public GetChatListResponse(string result, List<Chat> chatList)
        {
            Result = result;
            ChatList = chatList;
        }

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(GetChatListResponse),
                Payload = this
            };

            return container;
        }
    }
}
