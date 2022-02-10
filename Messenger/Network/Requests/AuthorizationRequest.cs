namespace Messenger.Network.Requests
{
    public class AuthorizationRequest
    {
        #region Properties

        public string Name { get; set; }

        #endregion //Properties

        #region Constructors

        public AuthorizationRequest(string name)
        {
            Name = name;
        }

        #endregion //Constructors

        #region Methods

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(AuthorizationRequest),
                Payload = this
            };

            return container;
        }

        #endregion //Methods
    }
}
