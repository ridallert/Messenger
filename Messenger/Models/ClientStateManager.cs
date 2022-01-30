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
        private int? _userId;

        private List<User> _contacts;
        private List<Chat> _chats;
        private List<Message> _publicMessageList;
        private List<LogEntry> _eventList;

        public string Login
        {
            get { return _login; }
            set { _login = value; }
        }
        public int? UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }
        public List<User> Contacts
        {
            get { return _contacts; }
            set { _contacts = value; }
        }
        public List<Chat> Chats
        {
            get { return _chats; }
            set { _chats = value; }
        }
        public List<Message> PublicMessageList
        {
            get { return _publicMessageList; }
            set { _publicMessageList = value; }
        }
        public List<LogEntry> EventList
        {
            get { return _eventList; }
            set { _eventList = value; }
        }

        public event Action ContactListChanged;
        public event Action ChatListChanged;
        public event Action UserAuthorized;
        public event Action UserLoggedOut;
        public event Action<Message> NewMessageAdded;
        public event Action PublicMessageListChanged;
        public event Action EventListChanged;

        public ClientStateManager(WebSocketClient webSocketClient)
        {
            Contacts = new List<User>();
            Chats = new List<Chat>();
            PublicMessageList = new List<Message>();

            _webSocketClient = webSocketClient;
            _webSocketClient.AuthorizationResponseСame += AuthorizeUser;
            _webSocketClient.GetContactsResponseСame += LoadContactList;
            _webSocketClient.GetChatListResponseСame += LoadChatList;
            //_webSocketClient.GetPrivateMessageListResponseCame += LoadPrivateMessageList;
            _webSocketClient.GetPublicMessageListResponseCame += LoadPublicMessageList;
            //_webSocketClient.PrivateMessageReceivedResponseCame += AddPrivateMessage;
            _webSocketClient.PublicMessageReceivedResponseCame += AddPublicMessage;
            //_webSocketClient.UserStatusChangedBroadcastCame += OnUserStatusChangedBroadcastCame;
            _webSocketClient.GetEventListResponseCame += LoadEventLog;
        }
        public void AuthorizeUser(AuthorizationResponse response)
        {
            if (response.Result == "AlreadyExists" || response.Result == "NewUserAdded")
            {
                Login = response.Name;
                UserId = response.UserId;
                UserAuthorized?.Invoke();
            }
        }
        public void LogOut()
        {
            Login = null;
            UserLoggedOut?.Invoke();
        }
        private void LoadContactList(GetContactsResponse getContactsResponse)
        {
            Contacts = new List<User>(getContactsResponse.ContactList);
            ContactListChanged?.Invoke();
        }
        private void LoadChatList(GetChatListResponse getChatListResponse)
        {
            //foreach (IChat chat in getChatListResponse.ChatList)
            //{
            //    if (chat is GroupChat)
            //    {
            //        ((GroupChat)chat).Title = chat.Users.Find(user => user.Name != Login).Name;
            //    }
            //    Chats.Add(chat);
            //}
            Chats = new List<Chat>(getChatListResponse.ChatList);
            ChatListChanged?.Invoke();
        }
        public ObservableCollection<User> GetContactList()
        {
            return new ObservableCollection<User>(Contacts);
        }
        public ObservableCollection<ChatPresenter> GetChatList()
        {
            ObservableCollection<ChatPresenter> result = new ObservableCollection<ChatPresenter>();
  
            foreach (Chat chat in Chats)
            {
                result.Add(chat.ToChatPresenter(chat, Login));
            }

            return result;
        }
        //public void LoadPrivateMessageList(GetPrivateMessageListResponse response)
        //{
        //    foreach (Contact contact in Contacts)
        //    {
        //        foreach (Message message in response.MessageList)
        //        {
        //            if ((contact.Title == message.Sender || contact.Title == message.Receiver) && contact.Users.Count == 1)
        //            {
        //                contact.MessageList.Add(message);
        //            }
        //        }
        //    }
        //}
        public void LoadPublicMessageList(GetPublicMessageListResponse response)
        {
            PublicMessageList = new List<Message>(response.MessageList);
            PublicMessageListChanged?.Invoke();
        }

        //public void OnUserStatusChangedBroadcastCame(UserStatusChangedBroadcast broadcast)
        //{
        //    bool isUserExist = false;

        //    foreach (Contact contact in Contacts)
        //    {
        //        if (contact.Title == broadcast.Name && contact.Users.Count == 1)
        //        {
        //            contact.IsOnline = broadcast.Status;
        //            isUserExist = true;
        //        }
        //    }
        //    if (!isUserExist)
        //    {
        //        if (broadcast.Status == OnlineStatus.Online)
        //        {
        //            Application.Current.Dispatcher.InvokeAsync(() =>
        //            {
        //                Contact newChat = new Contact(broadcast.Name);
        //                newChat.IsOnline = broadcast.Status;
        //                Contacts.Add(newChat);
        //                ContactListChanged?.Invoke();
        //            });
        //        }
        //    }
        //}

        public ObservableCollection<Message> GetMessageList(int chatId)
        {
            List<Message> messages = new List<Message>();

            messages = Chats.Find(chat => chat.ChatId == chatId).Messages;
            

            return new ObservableCollection<Message>(messages);
        }
        //public ObservableCollection<Message> GetPublicMessageList()
        //{
        //    return new ObservableCollection<Message>(PublicMessageList);
        //}

        //public void AddPrivateMessage(PrivateMessageReceivedResponse response)
        //{
        //    Application.Current.Dispatcher.InvokeAsync(() =>
        //    {
        //        foreach (Contact contact in Contacts)
        //        {
        //            if ((contact.Title == response.Sender || contact.Title == response.Receiver) && contact.Users.Count == 1)
        //            {
        //                Message message = new Message(response.Sender, response.Receiver, response.Text, response.SendTime);
        //                contact.MessageList.Add(message);
        //                NewMessageAdded?.Invoke(message);
        //            }
        //        }
        //    });
        //}
        public void AddPublicMessage(PublicMessageReceivedResponse response)
        {
            //Application.Current.Dispatcher.InvokeAsync(() =>
            //{
            //    Message message = new Message(response.Sender, "Public chat", response.Text, response.SendTime);
            //    PublicMessageList.Add(message);
            //    NewMessageAdded?.Invoke(message);
            //    PublicMessageListChanged?.Invoke();
            //});
        }

        //public void SetUserOnline(string name)
        //{
        //    foreach (Contact contact in Contacts)
        //    {
        //        if (contact.Title == name && contact.Users.Count == 1)
        //        {
        //            contact.IsOnline = OnlineStatus.Online;
        //        }
        //    }
        //}
        private void LoadEventLog(GetEventListResponse response)
        {
            EventList = response.EventList;
            EventListChanged?.Invoke();
        }
        public ObservableCollection<LogEntry> GetEventLog(EventType type)
        {
            if (type == EventType.All)
            {
                return new ObservableCollection<LogEntry>(EventList);
            }
            else
            {
                return new ObservableCollection<LogEntry>(EventList.FindAll(entry => entry.Type == type));
            }
        }
    }
}
