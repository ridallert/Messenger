using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Network.Responses
{
    public class AuthorizationResponse
    {
        public string Result { get; set; }
        public string Name { get; set; }

        public AuthorizationResponse(string result, string name)
        {
            Result = result;
            Name = name;
        }

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(AuthorizationResponse),
                Payload = this
            };

            return container;
        }
    }
}
