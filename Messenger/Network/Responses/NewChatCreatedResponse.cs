namespace Messenger.Network.Responses
{
    using Messenger.DataObjects;

    public class NewChatCreatedResponse
    {
        #region Properties

        public Chat Chat { get; set; }

        #endregion //Properties

        #region Constructors

        public NewChatCreatedResponse(Chat chat)
        {
            Chat = chat;
        }

        #endregion //Constructors

        #region Methods

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(NewChatCreatedResponse),
                Payload = this
            };

            return container;
        }

        #endregion //Methods
    }
}
