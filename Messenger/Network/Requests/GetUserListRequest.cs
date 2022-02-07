using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Network.Requests
{
    class GetUserListRequest
    {
        public int UserId { get; set; }

        public GetUserListRequest(int userId)
        {
            UserId = userId;
        }
        public MessageContainer GetContainer()
        {

            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(GetUserListRequest),
                Payload = this
            };

            return container;
        }
    }
}
