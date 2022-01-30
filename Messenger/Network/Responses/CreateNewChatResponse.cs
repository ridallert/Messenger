using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Network.Responses
{
    public class CreateNewChatResponse
    {
        public string Result { get; set; }

        public CreateNewChatResponse(string result)
        {
            Result = result;
        }

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(CreateNewChatResponse),
                Payload = this
            };

            return container;
        }
    }
}
