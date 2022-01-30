using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Network.Requests
{
    public class CreateNewChatRequest
    {
        public string Title { get; set; }
        public List<int> UserIdList { get; set; }

        public CreateNewChatRequest(string title, List<int> userIdList)
        {
            Title = title;
            UserIdList = userIdList;
        }

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(CreateNewChatRequest),
                Payload = this
            };

            return container;
        }
    }
}
