namespace Messenger.Network.Requests
{
    public class GetChatListRequest
    {
        #region Properties

        public int UserId { get; set; }

        #endregion //Properties

        #region Constructors

        public GetChatListRequest(int userId)
        {
            UserId = userId;
        }

        #endregion //Constructors

        #region Methods

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(GetChatListRequest),
                Payload = this
            };

            return container;
        }

        #endregion //Methods
    }
}
