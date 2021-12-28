using Messenger.Common;
using Messenger.Network;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Models
{
    public class ClientState
    {
        private ObservableCollection<User> _users;
        private User _authorizedUser;

        public ObservableCollection<User> Users
        {
            get { return _users; }
            set
            {
                _users = value;
                UserListChanged?.Invoke();
            }
        }
        public User AuthorizedUser
        {
            get { return _authorizedUser; }
            set
            {
                _authorizedUser = value;
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

        public event Action UserListChanged;
        public event Action UserAuthorized;
        public event Action UserLoggedOut;


        public ClientState()
        {
            Users = new ObservableCollection<User>();

            //Users.Add(new User("Евгений", OnlineStatus.Offline));
            //Users.Add(new User("Яков", OnlineStatus.Offline));
            //Users.Add(new User("Виктория", OnlineStatus.Online));
            //Users.Add(new User("Мария", OnlineStatus.Offline));
            //Users.Add(new User("Ридаль", OnlineStatus.Offline));
        }

        public ObservableCollection<User> GetContacts(User me)
        {
            ObservableCollection<User> contactList = new ObservableCollection<User>();
            for (int i = 0; i < Users.Count; i++)
            {
                if (me != null && me.Name != Users[i].Name)
                {
                    contactList.Add(Users[i]);
                }
            }
            return contactList;
        }

        public ObservableCollection<Message> GetMessageList(User me, User contact)
        {
            ObservableCollection<Message> messages = new ObservableCollection<Message>();

            for (int i = 0; i < me.MessageList.Count; i++)
            {
                bool isF = ((me.MessageList[i].Sender.Name == contact.Name && me.MessageList[i].Receiver.Name == me.Name) ||
                            (me.MessageList[i].Sender.Name == me.Name && me.MessageList[i].Receiver.Name == contact.Name)) &&
                            me.MessageList[i].IsGroopChatMessage == false;

                if (isF)
                {
                    messages.Add(me.MessageList[i]);
                }
            }

            return messages;
        }

        public ObservableCollection<Message> GetGroupMessageList(User me)
        {
            ObservableCollection<Message> messages = new ObservableCollection<Message>();

            for (int i = 0; i < me.MessageList.Count; i++)
            {
                if (me.MessageList[i].IsGroopChatMessage == true)
                {
                    messages.Add(me.MessageList[i]);
                }
            }
            return messages;
        }

        public void SendMessage(User sender, User receiver, string text)
        {
            for (int i = 0; i < Users.Count; i++)
            {
                if (receiver.Name == Users[i].Name || sender.Name == Users[i].Name)
                {
                    Users[i].MessageList.Add(new Message(sender, receiver, text));
                }
            }
        }

        public void AddNewUser(string name)
        {
            AuthorizedUser = new User(name, OnlineStatus.Online);
            //Users.Add(AuthorizedUser);
        }
        public void SetUserOnline(string name)
        {
            //for (int i = 0; i < Users.Count; i++)
            //{
            //    if (Users[i].Name == user.Name)
            //    {
            //        isUserAlreadyExists = true;
            //        Users[i].IsOnline = OnlineStatus.Online;
            //        user = Users[i];
            //    }
            //}
        }

        //public void AuthorizeUser(User user)
        //{
        //    bool isUserAlreadyExists = false;

        //    for (int i = 0; i < Users.Count; i++)
        //    {
        //        if (Users[i].Name == user.Name)
        //        {
        //            isUserAlreadyExists = true;
        //            Users[i].IsOnline = OnlineStatus.Online;
        //            user = Users[i];
        //        }
        //    }

        //    if (isUserAlreadyExists == false)
        //    {
        //        Users.Add(user);
        //    }

        //    AuthorizedUser = user;
        //}

        public void SendGroupMessage(User sender, string text)
        {
            for (int i = 0; i < Users.Count; i++)
            {
                Users[i].MessageList.Add(new Message(sender, Users[i], text, true));
            }
        }
    }
}
