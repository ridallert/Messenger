namespace Messenger.Network.Requests
{
    using System;

    public class GetEventListRequest
    {
        #region Properties

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        #endregion //Properties

        #region Constructors

        public GetEventListRequest(DateTime from, DateTime to)
        {
            From = from;
            To = to;
        }

        #endregion //Constructors

        #region Methods

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(GetEventListRequest),
                Payload = this
            };

            return container;
        }

        #endregion //Methods
    }
}
