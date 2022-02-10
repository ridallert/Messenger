namespace Messenger.Network.Responses
{
    using System.Collections.Generic;
    using Messenger.DataObjects;

    public class GetUserListResponse
    {
        #region Properties

        public string Result { get; set; }

        public List<User> ContactList { get; set; }

        #endregion //Properties

        #region Constructors

        public GetUserListResponse(string result, List<User> contactList)
        {
            Result = result;
            ContactList = contactList;
        }

        #endregion //Constructors

        #region Methods

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(GetUserListResponse),
                Payload = this
            };

            return container;
        }

        #endregion //Methods
    }
}
