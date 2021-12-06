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
        private string name;
        private OnlineStatus isOnline;
        private ObservableCollection<Message> messageList;

        public string Name
        {
            get { return name; }
            set
            {
                if (value != name)
                {
                    SetProperty<string>(ref name, value);
                }
            }
        }
        public OnlineStatus IsOnline
        {
            get { return isOnline; }
            set
            {
                if (value != isOnline)
                {
                    SetProperty<OnlineStatus>(ref isOnline, value);
                }
            }
        }
        public ObservableCollection<Message> MessageList { get; set; }

        public User(string name, OnlineStatus isOnline)
        {
            Name = name;
            IsOnline = isOnline;
            MessageList = new ObservableCollection<Message>();
        }
    }
}
