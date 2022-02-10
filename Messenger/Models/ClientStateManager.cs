namespace Messenger.Models
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows;

    using Messenger.DataObjects;
    using Messenger.Network;
    using Messenger.Network.Broadcasts;
    using Messenger.Network.Responses;

    public class ClientStateManager
    {
        #region Fields

        private readonly WebSocketClient _webSocketClient;

        #endregion //Fields

        #region Properties

        public string Login { get; set; }

        public int? UserId { get; set; }

        public List<User> Users { get; set; }

        public List<Chat> Chats { get; set; }

        public List<LogEntry> EventList { get; set; }

        #endregion //Properties

        #region Events

        public event Action<User> UserStatusChanged;
        public event Action UserListChanged;
        public event Action ChatListLoaded;
        public event Action<ChatPresenter> NewChatAdded;
        public event Action UserAuthorized;
        public event Action UserLoggedOut;
        public event Action<Message> MessageReceived;
        public event Action EventListChanged;

        #endregion //Events

        #region Constructors

        public ClientStateManager(WebSocketClient webSocketClient)
        {
            Users = new List<User>();
            Chats = new List<Chat>();

            _webSocketClient = webSocketClient;
            _webSocketClient.AuthorizationResponseСame += AuthorizeUser;
            _webSocketClient.UserStatusChangedBroadcastCame += ChangeUserStatus;
            _webSocketClient.GetUserListResponseСame += LoadUserList;
            _webSocketClient.GetChatListResponseСame += LoadChatList;
            _webSocketClient.NewChatCreatedResponseСame += AddNewChat;
            _webSocketClient.MessageReceivedResponseCame += AddMessage;
            _webSocketClient.GetEventListResponseCame += LoadEventLog;
        }

        #endregion //Constructors

        #region Methods

        public void LogOut()
        {
            Login = null;
            UserLoggedOut?.Invoke();
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

        public ObservableCollection<Message> GetMessageList(int chatId)
        {
            List<Message> messages = Chats.Find(chat => chat.ChatId == chatId).Messages;

            return new ObservableCollection<Message>(messages);
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

        public bool IsChatAlreadyExists(ObservableCollection<User> userList)
        {
            foreach (Chat existingChat in Chats)
            {
                int userCounter = 0;
                foreach (User existingChatUser in existingChat.Users)
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
                if (userCounter == userList.Count + 1 && userCounter == existingChat.Users.Count) //userList.Count != 1 &&
                {
                    if (existingChat.Title == "Public chat" && existingChat.Users.Count == 2)
                    {
                        return false;
                    }
                    return true;
                }
            }
            return false;
        }

        private void AuthorizeUser(AuthorizationResponse response)
        {
            if (response.Result == "Success")
            {
                Login = response.Name;
                UserId = response.UserId;
                UserAuthorized?.Invoke();
            }
        }

        private void LoadUserList(GetUserListResponse getContactsResponse)
        {
            Users = new List<User>(getContactsResponse.ContactList);
            UserListChanged?.Invoke();
        }

        private void LoadChatList(GetChatListResponse getChatListResponse)
        {
            Chats = new List<Chat>(getChatListResponse.ChatList);
            ChatListLoaded?.Invoke();
        }

        private void AddNewChat(NewChatCreatedResponse response)
        {
            Chats.Add(response.Chat);
            NewChatAdded?.Invoke(response.Chat.ToChatPresenter(Login));
        }

        private void ChangeUserStatus(UserStatusChangedBroadcast broadcast)
        {
            bool isUserExist = false;

            foreach (User user in Users)
            {
                if (user.UserId == broadcast.UserId)
                {
                    user.IsOnline = broadcast.Status;
                    isUserExist = true;
                    UserStatusChanged?.Invoke(user);
                }
            }

            if (!isUserExist)
            {
                if (broadcast.Status == UserStatus.Online)
                {
                    Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        User newUser = new User(broadcast.UserId, broadcast.Name, broadcast.Status);
                        Users.Add(newUser);
                        Chats.Find(chat => chat.Title == "Public chat")?.Users.Add(newUser);
                        UserStatusChanged?.Invoke(newUser);
                    });
                }
            }
        }

        private void AddMessage(MessageReceivedResponse response)
        {
            Chat targetChat = Chats.Find(chat => chat.ChatId == response.ChatId);
            {
                if (targetChat != null)
                {
                    Message message = new Message(response.MessageId, response.SenderId, response.ChatId, response.SenderName, response.Text, response.SendTime);
                    targetChat.Messages.Add(message);
                    MessageReceived?.Invoke(message);
                }
            }
        }

        private void LoadEventLog(GetEventListResponse response)
        {
            EventList = response.EventList;
            EventListChanged?.Invoke();
        }

        #endregion //Methods
    }
}