using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Network.Requests
{
    class AuthorizationRequest
    {
        public string Name { get; set; }

        public AuthorizationRequest(string name)
        {
            Name = name;
        }

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(AuthorizationRequest),
                Payload = this
            };

            return container;
        }
    }
}
