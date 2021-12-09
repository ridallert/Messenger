using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Models
{
    public class Message
    {
        private User _sender;
        private User _receiver;
        private string _text;
        private DateTime _sendTime;
        private bool _isGroopchatMessage;

        public User Sender
        {
            get { return _sender; }
            set { _sender = value; }
        }
        public User Receiver
        {
            get { return _receiver; }
            set { _receiver = value; }
        }
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }
        public DateTime SendTime
        {
            get { return _sendTime; }
            set { _sendTime = value; }
        }
        public bool IsGroopChatMessage
        {
            get { return _isGroopchatMessage; }
            set { _isGroopchatMessage = value; }
        }

        public Message(User sender, User receiver, string text, bool isGroopChatMess = false)
        {
            Sender = sender;
            Receiver = receiver;
            Text = text;
            SendTime = DateTime.Now;
            IsGroopChatMessage = isGroopChatMess;
        }
    }
}
