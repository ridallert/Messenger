using Messenger.Common;
using System.Collections.Generic;

namespace Messenger.Network.Responses
{
    public class GetPublicMessageListResponse
    {
        public string Result { get; set; }
        public List<Message> MessageList { get; set; }

        public GetPublicMessageListResponse(string result, List<Message> messageList)
        {
            Result = result;
            MessageList = messageList;
        }

        //public GetPublicMessageListResponce(string result)
        //{
        //    Result = result;
        //    MessageList = new List<Message>();
        //}

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(GetPublicMessageListResponse),
                Payload = this
            };

            return container;
        }
    }
}
