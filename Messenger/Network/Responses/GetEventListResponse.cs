namespace Messenger.Network.Responses
{
    using System.Collections.Generic;

    using Messenger.DataObjects;

    public class GetEventListResponse
    {
        #region Properties

        public string Result { get; set; }

        public List<LogEntry> EventList {get;set;}

        #endregion //Properties

        #region Constructors

        public GetEventListResponse(string result, List<LogEntry> eventList)
        {
            Result = result;
            EventList = eventList;
        }

        #endregion //Constructors

        #region Methods

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(GetEventListResponse),
                Payload = this
            };

            return container;
        }

        #endregion //Methods
    }
}
