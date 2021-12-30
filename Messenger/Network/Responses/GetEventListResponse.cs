using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messenger.Common;

namespace Messenger.Network.Responses
{
    class GetEventListResponse
    {
        public string Result { get; set; }
        public List<LogEntry> EventList {get;set;}

        public GetEventListResponse(string result, List<LogEntry> eventList)
        {
            Result = result;
            EventList = eventList;
        }

        //public GetEventListResponce(string result)
        //{
        //    Result = result;
        //    EventList = new List<LogEntry>();
        //}

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(GetEventListResponse),
                Payload = this
            };

            return container;
        }
    }
}
