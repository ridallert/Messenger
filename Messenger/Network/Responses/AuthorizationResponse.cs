using Newtonsoft.Json;
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
        public int UserId { get; set; }

        [JsonConstructor]
        public AuthorizationResponse(string result, string name, int userId) : this(result)
        {
            Name = name;
            UserId = userId;
        }
        public AuthorizationResponse(string result)
        {
            Result = result;
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
