namespace Messenger.Common
{
    using Newtonsoft.Json;
    using System;
    public class Message
    {
        private static int _idCounter;

        public int MessageId { get; set; }
        public User Sender { get; set; }
        public int ChatId { get; set; }
        public string Text { get; set; }
        public DateTime SendTime { get; set; }

        [JsonConstructor]
        public Message(int messageId, User sender, int chatId, string text, DateTime sendTime) : this(sender, chatId, text, sendTime)
        {
            MessageId = messageId;
        }
        public Message(User sender, int chatId, string text, DateTime sendTime) : this()
        {
            Sender = sender;
            ChatId = chatId;
            Text = text;
            SendTime = sendTime;
        }
        public Message()
        {
            MessageId = _idCounter++;
        }
    }
}
