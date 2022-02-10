namespace Messenger.Network.Responses
{
    public class CreateNewChatResponse
    {
        #region Properties

        public string Result { get; set; }

        #endregion //Properties

        #region Constructors

        public CreateNewChatResponse(string result)
        {
            Result = result;
        }

        #endregion //Constructors

        #region Methods

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(CreateNewChatResponse),
                Payload = this
            };

            return container;
        }

        #endregion //Methods
    }
}
