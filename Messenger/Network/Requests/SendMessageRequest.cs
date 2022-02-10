namespace Messenger.Network.Requests
{
    using System;

    public class SendMessageRequest
    {
        #region Properties

        public int SenderId { get; set; }

        public int ChatId { get; set; }

        public string Text { get; set; }

        public DateTime SendTime { get; set; }

        #endregion //Properties

        #region Constructors

        public SendMessageRequest(int senderId, int chatId, string text, DateTime sendTime)
        {
            SenderId = senderId;
            ChatId = chatId;
            Text = text;
            SendTime = sendTime;
        }

        #endregion //Constructors

        #region Methods

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(SendMessageRequest),
                Payload = this
            };

            return container;
        }

        #endregion //Methods
    }
}
