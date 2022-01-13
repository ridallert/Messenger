using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Common
{
    public class Message
    {
        private string _sender;
        private string _receiver;
        private string _text;
        private DateTime _sendTime;

        public string Sender
        {
            get { return _sender; }
            set { _sender = value; }
        }
        public string Receiver
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

        public Message(string sender, string receiver, string text, DateTime sendTime)
        {
            Sender = sender;
            Receiver = receiver;
            Text = text;
            SendTime = sendTime;
        }
    }
}
