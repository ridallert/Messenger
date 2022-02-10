namespace Messenger.Network.Responses
{
    public class SendMessageResponse
    {
        #region Properties

        public string Result { get; set; }

        #endregion //Properties

        #region Constructors

        public SendMessageResponse(string result)
        {
            Result = result;
        }

        #endregion //Constructors

        #region Methods

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(SendMessageResponse),
                Payload = this
            };

            return container;
        }

        #endregion //Methods
    }
}
