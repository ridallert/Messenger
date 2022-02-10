namespace Messenger.DataObjects
{
    using System;

    using Newtonsoft.Json;

    public class Message
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

        [JsonConstructor]
        public Message(int messageId, int senderId, int chatId, string senderName, string text, DateTime sendTime)
        {
            MessageId = messageId;
            SenderId = senderId;
            ChatId = chatId;
            SenderName = senderName;
            Text = text;
            SendTime = sendTime;
        }

        #endregion //Constructors
    }
}
