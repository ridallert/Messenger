using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Models
{
    public class State : IState
    {
        private ObservableCollection<User> _users;
        private User _authorizedUser;

        public ObservableCollection<User> Users
        {
            get { return _users; }
            set
            {
                _users = value;
                if (UserListChanged != null)
                {
                    UserListChanged();
                }
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


        public State()
        {
            Users = new ObservableCollection<User>();

            Users.Add(new User("Евгений", OnlineStatus.Offline));
            Users.Add(new User("Яков", OnlineStatus.Offline));
            Users.Add(new User("Виктория", OnlineStatus.Offline));
            Users.Add(new User("Мария", OnlineStatus.Offline));
            Users.Add(new User("Ридаль", OnlineStatus.Offline));
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

            for (int i = 0; i < Users.Count; i++)
            {
                if (Users[i].Name == me.Name)
                {
                    for (int j = 0; j < Users[i].MessageList.Count; j++)
                    {
                        if (((Users[i].MessageList[j].Sender.Name == contact.Name && Users[i].MessageList[j].Receiver.Name == me.Name) ||
                            (Users[i].MessageList[j].Sender.Name == me.Name && Users[i].MessageList[j].Receiver.Name == contact.Name)) &&
                            Users[i].MessageList[j].IsGroopChatMessage == false)
                        {
                            messages.Add(Users[i].MessageList[j]);
                        }
                    }
                }
            }

            return messages;
        }

        public ObservableCollection<Message> GetGroupMessageList(User me)
        {
            ObservableCollection<Message> messages = new ObservableCollection<Message>();

            for (int i = 0; i < Users.Count; i++)
            {
                if (Users[i].Name == me.Name)
                {
                    for (int j = 0; j < Users[i].MessageList.Count; j++)
                    {
                        if (Users[i].MessageList[j].IsGroopChatMessage == true)
                        {
                            messages.Add(Users[i].MessageList[j]);
                        }
                    }
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

        public void SendGroupMessage(User sender, string text)
        {
            for (int i = 0; i < Users.Count; i++)
            {
                Users[i].MessageList.Add(new Message(sender, Users[i], text, true));
            }
        }
    }
}
