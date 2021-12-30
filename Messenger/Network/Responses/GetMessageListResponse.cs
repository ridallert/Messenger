using Messenger.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Network.Responses
{
    public class GetMessageListResponse
    {
        public string Result { get; set; }
        public List<Message> MessageList { get; set; }

        public GetMessageListResponse(string result, List<Message> messageList)
        {
            Result = result;
            MessageList = messageList;
        }

        //public GetMessageListResponse(string result)
        //{
        //    Result = result;
        //    MessageList = new List<Message>();
        //}

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(GetMessageListResponse),
                Payload = this,
            };

            return container;
        }
    }
}
