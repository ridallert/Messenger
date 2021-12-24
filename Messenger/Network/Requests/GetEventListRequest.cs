using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Network.Requests
{
    class GetEventListRequest
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public GetEventListRequest(DateTime from, DateTime to)
        {
            From = from;
            To = to;
        }

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(GetEventListRequest),
                Payload = this
            };

            return container;
        }
    }
}
