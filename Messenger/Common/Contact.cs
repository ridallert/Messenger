using Messenger.Network;
using Newtonsoft.Json;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Common
{
    public class Contact : BindableBase
    {
        private string _title;
        private List<string> _users;
        private OnlineStatus _isOnline;
        private ObservableCollection<Message> _messageList;
        private int? _newMessageCounter;

        public string Title
        {
            get { return _title; }
            set
            {
                if (value != _title)
                {
                    SetProperty(ref _title, value);
                }
            }
        }
        public List<string> Users
        {
            get { return _users; }
            set
            {
                if (value != _users)
                {
                    SetProperty(ref _users, value);
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
                    SetProperty(ref _isOnline, value);
                }
            }
        }
        public ObservableCollection<Message> MessageList
        {
            get { return _messageList; }
            set
            {
                SetProperty(ref _messageList, value);
            }
        }
        public int? NewMessageCounter
        {
            get { return _newMessageCounter; }
            set
            {
                if (value != _newMessageCounter)
                {
                    SetProperty(ref _newMessageCounter, value);
                }
            }
        }
       
        public Contact(string user)
        {
            Title = user;
            Users = new List<string>();
            Users.Add(user);
            MessageList = new ObservableCollection<Message>();
        }

        [JsonConstructor]
        public Contact(string chatName, List<string> users)
        {
            Title = chatName;
            Users = users;
            MessageList = new ObservableCollection<Message>();
        }
    }
}
