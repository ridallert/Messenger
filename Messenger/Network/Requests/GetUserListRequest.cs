namespace Messenger.Network.Requests
{
    public class GetUserListRequest
    {
        #region Properties

        public int UserId { get; set; }

        #endregion //Properties

        #region Constructors

        public GetUserListRequest(int userId)
        {
            UserId = userId;
        }

        #endregion //Constructors

        #region Methods

        public MessageContainer GetContainer()
        {

            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(GetUserListRequest),
                Payload = this
            };

            return container;
        }

        #endregion //Methods
    }
}
