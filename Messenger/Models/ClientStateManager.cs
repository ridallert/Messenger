namespace Messenger.Models
{
    using Messenger.Common;
    using Messenger.Network;
    using Messenger.Network.Broadcasts;
    using Messenger.Network.Responses;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows;

    public class ClientStateManager
    {
        private WebSocketClient _webSocketClient;

        public string Login { get; set; }
        public int? UserId { get; set; }
        public List<User> Users { get; set; }
        public List<Chat> Chats { get; set; }
        public List<Message> PublicMessageList { get; set; }
        public List<LogEntry> EventList { get; set; }

        public event Action<User> UserStatusChanged;
        public event Action UserListChanged;
        public event Action ChatListLoaded;
        public event Action<ChatPresenter> NewChatAdded;
        public event Action UserAuthorized;
        public event Action UserLoggedOut;
        public event Action<Message> MessageReceived;
        public event Action EventListChanged;

        public ClientStateManager(WebSocketClient webSocketClient)
        {
            Users = new List<User>();
            Chats = new List<Chat>();
            PublicMessageList = new List<Message>();

            _webSocketClient = webSocketClient;
            _webSocketClient.AuthorizationResponseСame += AuthorizeUser;
            _webSocketClient.UserStatusChangedBroadcastCame += ChangeUserStatus;
            _webSocketClient.GetUserListResponseСame += LoadUserList;
            _webSocketClient.GetChatListResponseСame += LoadChatList;
            _webSocketClient.NewChatCreatedResponseСame += AddNewChat;
            _webSocketClient.MessageReceivedResponseCame += AddMessage;
            _webSocketClient.GetEventListResponseCame += LoadEventLog;
        }

        public void AuthorizeUser(AuthorizationResponse response)
        {
            if (response.Result == "Already exists" || response.Result == "New user added")
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
                    if (existingChat.Title == "Public chat" && existingChat.Users.Count == 1)
                    {
                        return false;
                    }
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

        public void ChangeUserStatus(UserStatusChangedBroadcast broadcast)
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
            List<Message> messages = Chats.Find(chat => chat.ChatId == chatId).Messages;

            return new ObservableCollection<Message>(messages);
        }

        public void AddMessage(MessageReceivedResponse response)
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