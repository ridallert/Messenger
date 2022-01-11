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
        private ObservableCollection<User> _contacts;
        private ObservableCollection<Message> _publicMessageList;
        public string Login
        {
            get { return _login; }
            set
            {
                _login = value;
                if (value != null)
                {
                    UserAuthorized();
                }
                else
                {
                    UserLoggedOut();
                }
            }
        }
        public ObservableCollection<User> Contacts
        {
            get { return _contacts; }
            set
            {
                _contacts = value;
                ContactListChanged?.Invoke();
            }
        }
        public ObservableCollection<Message> PublicMessageList
        {
            get { return _publicMessageList; }
            set
            {
                _publicMessageList = value;
                PublicMessageListChanged?.Invoke();
            }
        }

        public event Action ContactListChanged;
        public event Action UserAuthorized;
        public event Action UserLoggedOut;
        public event Action<Message> NewMessageAdded;
        public event Action PublicMessageListChanged;

        public ClientStateManager(WebSocketClient webSocketClient)
        {
            Contacts = new ObservableCollection<User>();
            PublicMessageList = new ObservableCollection<Message>();

            _webSocketClient = webSocketClient;
            _webSocketClient.GetContactsResponseСame += LoadUserList;
            _webSocketClient.GetPrivateMessageListResponseCame += LoadPrivateMessageList;
            _webSocketClient.PrivateMessageReceivedResponseCame += AddPrivateMessage;
            _webSocketClient.UserStatusChangedBroadcastCame += OnUserStatusChangedBroadcastCame;           
        }
        public void AuthorizeUser(string login)
        {
            Login = login;
        }
        private void LoadUserList(GetContactsResponse getContactsResponse)
        {
            Contacts = new ObservableCollection<User>(getContactsResponse.UserList);
        }

        public void LoadPrivateMessageList(GetPrivateMessageListResponse getMessageListResponse)
        {
            foreach (User contact in Contacts)
            {
                foreach (Message message in getMessageListResponse.MessageList)
                {
                    if ((contact.Name == message.Sender || contact.Name == message.Receiver) && !message.IsGroopChatMessage)
                    {
                        contact.MessageList.Add(message);
                    }
                }
            }

        }


        public void OnUserStatusChangedBroadcastCame(UserStatusChangedBroadcast broadcast)
        {
            bool isUserExist = false;

            foreach (User contact in Contacts)
            {
                if (contact.Name == broadcast.Name)
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
                        Contacts.Add(new User(broadcast.Name, broadcast.Status));
                    });
                    
                }
                else ////////////////////////////////////////////////////////////////////////////////////////////////////
                {
                    throw new Exception("Пользователя с именем '" + broadcast.Name + "' на клиенте не зарегистрировано");
                }
            }
        }

        public ObservableCollection<Message> GetPrivateMessageList(string name)
        {
            ObservableCollection<Message> messages = new ObservableCollection<Message>();
            foreach (User contact in Contacts)
            {
                if (contact.Name == name)
                {
                    messages = contact.MessageList;
                }
            }
            return messages;
        }



        public void AddPrivateMessage(PrivateMessageReceivedResponse response)
        {
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                foreach (User contact in Contacts)
                {
                    if (contact.Name == response.Sender || contact.Name == response.Receiver)
                    {
                        Message message = new Message(response.Sender, response.Receiver, response.Text, response.SendTime);
                        contact.MessageList.Add(message);
                        NewMessageAdded?.Invoke(message);
                    }
                }
            });
        }

        public void SetUserOnline(string name)
        {
            foreach (User contact in Contacts)
            {
                if (contact.Name == name)
                {
                    contact.IsOnline = OnlineStatus.Online;
                }
            }
        }



        //public void SendGroupMessage(string sender, string text) переделать
        //{
        //    for (int i = 0; i < Users.Count; i++)
        //    {
        //        Users[i].MessageList.Add(new Message(sender, Users[i].Name, text, true));
        //    }
        //}



        public ObservableCollection<Message> GetPublicMessageList()
        {
            return PublicMessageList;
        }
}
}
