using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Models
{
    public class Message
    {
        private User sender;
        private User receiver;
        private string text;
        private DateTime sendTime;

        public User Sender
        {
            get { return sender; }
            set { sender = value; }
        }
        public User Receiver
        {
            get { return receiver; }
            set { receiver = value; }
        }
        public string Text
        {
            get { return text; }
            set { text = value; }
        }
        public DateTime SendTime
        {
            get { return sendTime; }
            set { sendTime = value; }
        }

        public Message(User sender, User reciever, string text, DateTime sendTime)
        {
            Sender = sender;
            Receiver = reciever;
            Text = text;
            SendTime = sendTime;
        }
    }
}
