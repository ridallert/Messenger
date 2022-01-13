using Messenger.Common;
using Messenger.Network;
using Messenger.Network.Broadcasts;
using Messenger.Network.Responses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Messenger.Models
{
    public class ClientStateManager
    {
        private WebSocketClient _webSocketClient;

        private string _login;
        private List<Contact> _contacts;
        private List<Message> _publicMessageList;
        private List<LogEntry> _eventList;
        public string Login
        {
            get { return _login; }
            set
            {
                _login = value;
                if (value != null)
                {
                    UserAuthorized?.Invoke();
                }
                else
                {
                    UserLoggedOut?.Invoke();
                }
            }
        }
        public List<Contact> Contacts
        {
            get { return _contacts; }
            set
            {
                _contacts = value;
                ContactListChanged?.Invoke();
            }
        }
        public List<Message> PublicMessageList
        {
            get { return _publicMessageList; }
            set
            {
                _publicMessageList = value;
                PublicMessageListChanged?.Invoke();
            }
        }
        public List<LogEntry> EventList
        {
            get { return _eventList; }
            set
            {
                _eventList = value;
                EventListChanged?.Invoke();
            }
        }

        public event Action ContactListChanged;
        public event Action UserAuthorized;
        public event Action UserLoggedOut;
        public event Action<Message> NewMessageAdded;
        public event Action PublicMessageListChanged;
        public event Action EventListChanged;

        public ClientStateManager(WebSocketClient webSocketClient)
        {
            Contacts = new List<Contact>();
            PublicMessageList = new List<Message>();

            _webSocketClient = webSocketClient;
            _webSocketClient.GetContactsResponseСame += LoadContactList;
            _webSocketClient.GetPrivateMessageListResponseCame += LoadPrivateMessageList;
            _webSocketClient.GetPublicMessageListResponseCame += LoadPublicMessageList;
            _webSocketClient.PrivateMessageReceivedResponseCame += AddPrivateMessage;
            _webSocketClient.PublicMessageReceivedResponseCame += AddPublicMessage;
            _webSocketClient.UserStatusChangedBroadcastCame += OnUserStatusChangedBroadcastCame;
            _webSocketClient.GetEventListResponseCame += LoadEventLog;
        }
        public void AuthorizeUser(string login)
        {
            Login = login;
        }
        private void LoadContactList(GetContactsResponse getContactsResponse)
        {
            Contacts = new List<Contact>(getContactsResponse.ContactList);
        }
        public ObservableCollection<Contact> GetContactList()
        {
            return new ObservableCollection<Contact>(Contacts);
        }
        public void LoadPrivateMessageList(GetPrivateMessageListResponse response)
        {
            foreach (Contact contact in Contacts)
            {
                foreach (Message message in response.MessageList)
                {
                    if ((contact.Title == message.Sender || contact.Title == message.Receiver) && contact.Users.Count == 1)
                    {
                        contact.MessageList.Add(message);
                    }
                }
            }

        }
        public void LoadPublicMessageList(GetPublicMessageListResponse response)
        {
            PublicMessageList = new List<Message>(response.MessageList);
        }

        public void OnUserStatusChangedBroadcastCame(UserStatusChangedBroadcast broadcast)
        {
            bool isUserExist = false;

            foreach (Contact contact in Contacts)
            {
                if (contact.Title == broadcast.Name && contact.Users.Count == 1)
                {
                    contact.IsOnline = broadcast.Status;
                    isUserExist = true;
                }
            }
            if (!isUserExist)
            {
                if (broadcast.Status == OnlineStatus.Online)
                {
                    Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        Contact newChat = new Contact(broadcast.Name);
                        newChat.IsOnline = broadcast.Status;
                        Contacts.Add(newChat);
                    });

                }
                else /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                {
                    throw new Exception("Пользователя или чата с именем '" + broadcast.Name + "' на клиенте не зарегистрировано");
                }
            }
        }

        public ObservableCollection<Message> GetPrivateMessageList(string name)
        {
            ObservableCollection<Message> messages = new ObservableCollection<Message>();
            foreach (Contact contact in Contacts)
            {
                if (contact.Title == name)
                {
                    messages = contact.MessageList;
                }
            }
            return messages;
        }
        public ObservableCollection<Message> GetPublicMessageList()
        {
            return new ObservableCollection<Message>(PublicMessageList);
        }

        public void AddPrivateMessage(PrivateMessageReceivedResponse response)
        {
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                foreach (Contact contact in Contacts)
                {
                    if ((contact.Title == response.Sender || contact.Title == response.Receiver) && contact.Users.Count == 1)
                    {
                        Message message = new Message(response.Sender, response.Receiver, response.Text, response.SendTime);
                        contact.MessageList.Add(message);
                        NewMessageAdded?.Invoke(message);
                    }
                }
            });
        }
        public void AddPublicMessage(PublicMessageReceivedResponse response)
        {
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Message message = new Message(response.Sender, "Public chat", response.Text, response.SendTime);
                PublicMessageList.Add(message);
                NewMessageAdded?.Invoke(message);
            });
        }

        public void SetUserOnline(string name)
        {
            foreach (Contact contact in Contacts)
            {
                if (contact.Title == name && contact.Users.Count == 1)
                {
                    contact.IsOnline = OnlineStatus.Online;
                }
            }
        }
        private void LoadEventLog(GetEventListResponse response)
        {
            EventList = response.EventList;
        }
        public ObservableCollection<LogEntry> GetEventLog()
        {
            return new ObservableCollection<LogEntry>(EventList);
        }

        //public void SendGroupMessage(string sender, string text) переделать
        //{
        //    for (int i = 0; i < Users.Count; i++)
        //    {
        //        Users[i].MessageList.Add(new Message(sender, Users[i].Name, text, true));
        //    }
        //}



        //public ObservableCollection<Message> GetPublicMessageList()
        //{
        //    return PublicMessageList;
        //}
    }
}
