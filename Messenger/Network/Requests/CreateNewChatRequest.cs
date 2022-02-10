namespace Messenger.Network.Requests
{
    using System.Collections.Generic;

    public class CreateNewChatRequest
    {
        #region Properties

        public string Title { get; set; }

        public List<int> UserIdList { get; set; }

        #endregion //Properties

        #region Constructors

        public CreateNewChatRequest(string title, List<int> userIdList)
        {
            Title = title;
            UserIdList = userIdList;
        }

        #endregion //Constructors

        #region Methods

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(CreateNewChatRequest),
                Payload = this
            };

            return container;
        }

        #endregion //Methods
    }
}
