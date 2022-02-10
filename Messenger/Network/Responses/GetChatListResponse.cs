namespace Messenger.Network.Responses
{
    using System.Collections.Generic;

    using Messenger.DataObjects;

    public class GetChatListResponse
    {
        #region Properties

        public string Result { get; set; }

        public List<Chat> ChatList { get; set; }

        #endregion //Properties

        #region Constructors

        public GetChatListResponse(string result, List<Chat> chatList)
        {
            Result = result;
            ChatList = chatList;
        }

        #endregion //Constructors

        #region Methods

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(GetChatListResponse),
                Payload = this
            };

            return container;
        }

        #endregion //Methods
    }
}
