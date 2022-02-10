namespace Messenger.Network.Responses
{
    using System;

    public class MessageReceivedResponse
    {
        #region Properties

        public int MessageId { get; set; }

        public int SenderId { get; set; }

        public int ChatId { get; set; }

        public string SenderName { get; set; }

        public string Text { get; set; }

        public DateTime SendTime { get; set; }

        #endregion //Properties

        #region Constructors

        public MessageReceivedResponse(int messageId, int senderId, int chatId, string senderName, string text, DateTime sendTime)
        {
            MessageId = messageId;
            SenderId = senderId;
            ChatId = chatId;
            SenderName = senderName;
            Text = text;
            SendTime = sendTime;
        }

        #endregion //Constructors

        #region Methods

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(MessageReceivedResponse),
                Payload = this
            };

            return container;
        }

        #endregion //Methods
    }
}
