using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Models
{
    public enum OnlineStatus
    {
        Online,
        Offline
    }
    public class User : BindableBase
    {
        private string _name;
        private OnlineStatus _isOnline;
        private ObservableCollection<Message> _messageList;

        public string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    SetProperty<string>(ref _name, value);
                }
            }
        }
        public OnlineStatus IsOnline
        {
            get { return _isOnline; }
            set
            {
                if (value != _isOnline)
                {
                    SetProperty<OnlineStatus>(ref _isOnline, value);
                }
            }
        }
        public ObservableCollection<Message> MessageList
        {
            get { return _messageList; }
            set
            {
                _messageList = value;
            }
        }

        public User(string name, OnlineStatus isOnline)
        {
            Name = name;
            IsOnline = isOnline;
            MessageList = new ObservableCollection<Message>();
        }

        //public static event Action MessageListChanged;
    }
}
