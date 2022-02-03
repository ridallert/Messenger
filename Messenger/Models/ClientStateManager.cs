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

        private List<User> _users;
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
        public List<User> Users
        {
            get { return _users; }
            set { _users = value; }
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

        public event Action<User> UserStatusChanged;
        public event Action UserListChanged;
        public event Action ChatListLoaded;
        public event Action<ChatPresenter> NewChatAdded;
        public event Action UserAuthorized;
        public event Action UserLoggedOut;
        public event Action<Message> MessageReceived;
        //public event Action PublicMessageListChanged;
        public event Action EventListChanged;

        public ClientStateManager(WebSocketClient webSocketClient)
        {
            Users = new List<User>();
            Chats = new List<Chat>();
            PublicMessageList = new List<Message>();

            _webSocketClient = webSocketClient;
            _webSocketClient.AuthorizationResponseСame += AuthorizeUser;
            _webSocketClient.UserStatusChangedBroadcastCame += ChangeUserStatus;
            _webSocketClient.GetUserListResponseСame += LoadContactList;
            _webSocketClient.GetChatListResponseСame += LoadChatList;
            _webSocketClient.NewChatCreatedResponseСame += AddNewChat;
            _webSocketClient.MessageReceivedResponseCame += AddMessage;
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
        private void LoadContactList(GetUserListResponse getContactsResponse)
        {
            Users = new List<User>(getContactsResponse.ContactList);
            UserListChanged?.Invoke();
        }
        private void LoadChatList(GetChatListResponse getChatListResponse)
        {
            Chats = new List<Chat>(getChatListResponse.ChatList);
            ChatListLoaded?.Invoke();
        }
        public ObservableCollection<User> GetContactList()
        {
            return new ObservableCollection<User>(Users);
        }
        public ObservableCollection<ChatPresenter> GetChatList()
        {
            ObservableCollection<ChatPresenter> result = new ObservableCollection<ChatPresenter>();

            foreach (Chat chat in Chats)
            {
                result.Add(chat.ToChatPresenter(Login));
            }

            return result;
        }
        private void AddNewChat(NewChatCreatedResponse response)
        {
            Chats.Add(response.Chat);
            NewChatAdded?.Invoke(response.Chat.ToChatPresenter(Login));
        }
        public bool IsChatAlreadyExists(ObservableCollection<User> userList)
        {
            foreach (Chat chat in Chats)
            {
                int userCounter = 0;
                foreach (User existingChatUser in chat.Users)
                {
                    foreach (User newChatUser in userList)
                    {
                        if (existingChatUser.UserId == newChatUser.UserId || existingChatUser.UserId == UserId)
                        {
                            userCounter++;
                            break;
                        }
                    }
                }
                if (userCounter == userList.Count + 1 && userCounter == chat.Users.Count)
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsChatNameTaken(string title)
        {
            foreach (Chat chat in Chats)
            {
                if (chat.Title == title)
                {
                    return true;
                }
            }
            return false;
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
        //public void LoadPublicMessageList(GetPublicMessageListResponse response)
        //{
        //    PublicMessageList = new List<Message>(response.MessageList);
        //    PublicMessageListChanged?.Invoke();
        //}

        public void ChangeUserStatus(UserStatusChangedBroadcast broadcast)
        {
            bool isUserExist = false;

            foreach (User user in Users)
            {
                if (user.Name == broadcast.Name)
                {
                    user.IsOnline = broadcast.Status;
                    isUserExist = true;
                    UserStatusChanged?.Invoke(user);
                }
            }
            if (!isUserExist)
            {
                if (broadcast.Status == OnlineStatus.Online)
                {
                    Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        User newUser = new User(broadcast.UserId, broadcast.Name, broadcast.Status);
                        Users.Add(newUser);
                        UserStatusChanged?.Invoke(newUser);
                    });
                }
            }
        }

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
        public void AddMessage(MessageReceivedResponse response)
        {
            //Application.Current.Dispatcher.InvokeAsync(() =>
            //{
            Chat targetChat = Chats.Find(chat => chat.ChatId == response.ChatId);
            {
                if (targetChat != null)
                {
                    Message message = new Message(response.MessageId, response.SenderId, response.ChatId, response.SenderName, response.Text, response.SendTime);
                    targetChat.Messages.Add(message);
                    MessageReceived?.Invoke(message);
                }
            }
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
