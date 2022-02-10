namespace Messenger.Network.Responses
{
    using Newtonsoft.Json;

    public class AuthorizationResponse
    {
        #region Properties

        public string Result { get; set; }

        public string Name { get; set; }

        public int UserId { get; set; }

        #endregion //Properties

        #region Constructors

        [JsonConstructor]
        public AuthorizationResponse(string result, string name, int userId) : this(result)
        {
            Name = name;
            UserId = userId;
        }

        public AuthorizationResponse(string result)
        {
            Result = result;
        }

        #endregion //Constructors

        #region Methods

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(AuthorizationResponse),
                Payload = this
            };

            return container;
        }

        #endregion //Methods
    }
}
