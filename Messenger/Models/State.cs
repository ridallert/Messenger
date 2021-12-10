using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Models
{
    public static class State
    {
        static private ObservableCollection<User> _users;
        static private User _authorizedUser;

        static public ObservableCollection<User> Users
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
        static public User AuthorizedUser
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

        //public delegate void AccountHandler(string message);
        //public static event AccountHandler Notify;

        public static event Action UserListChanged;
        public static event Action UserAuthorized;
        public static event Action UserLoggedOut;


        static State()
        {
            Users = new ObservableCollection<User>();

            //AuthorizedUser = new User("Ridal", OnlineStatus.Online);

            Users.Add(new User("Евгений", OnlineStatus.Offline));
            Users.Add(new User("Яков", OnlineStatus.Offline));
            Users.Add(new User("Виктория", OnlineStatus.Offline));
            Users.Add(new User("Мария", OnlineStatus.Offline));
            Users.Add(new User("Ридаль", OnlineStatus.Offline));

            //Users[0].MessageList.Add(new Message(Users[1], Users[0], "Привет Евгению от Якова", DateTime.Now));
            //Users[0].MessageList.Add(new Message(Users[1], Users[0], "Пока Евгению от Якова", DateTime.Now));
            //Users[0].MessageList.Add(new Message(Users[2], Users[0], "Привет Евгению от Виктории", DateTime.Now));
            //Users[0].MessageList.Add(new Message(Users[2], Users[0], "Пока Евгению от Виктории", DateTime.Now));

            //Users[0].MessageList.Add(new Message(Users[3], Users[0], "Привет Марии от Виктории", DateTime.Now));
            //Users[0].MessageList.Add(new Message(Users[3], Users[0], "Пока Марии от Виктории", DateTime.Now));

            //Users[1].MessageList.Add(new Message(Users[1], Users[0], "Привет Марии от Якова", DateTime.Now));

            //Users[0].MessageList.Add(new Message(Users[1], Users[0], "Привет Марии от Якова", DateTime.Now));
            //Users[0].MessageList.Add(new Message(Users[1], Users[0], "Пока Марии от Якова", DateTime.Now));
            //Users[0].MessageList.Add(new Message(Users[2], Users[0], "Привет Марии от Виктории", DateTime.Now));
            //Users[0].MessageList.Add(new Message(Users[2], Users[0], "Пока Марии от Виктории", DateTime.Now));

            //Users[0].MessageList.Add(new Message(Users[3], Users[0], "Привет Марии от Виктории", DateTime.Now));
            //Users[0].MessageList.Add(new Message(Users[3], Users[0], "Пока Марии от Виктории", DateTime.Now));


            //for (int i = 0; i < Users.Count; i++)
            //{
            //    Users[i].MessageList = new ObservableCollection<Message>
            //    {
            //        new Message(Users[i], AuthorizedUser, AuthorizedUser.Name + ", Привет! Это " + Users[i].Name, DateTime.Now),
            //        new Message(Users[i], AuthorizedUser, "Еще раз привет!", DateTime.Now),
            //        new Message(Users[i], AuthorizedUser, "Пока", DateTime.Now)
            //    };
            //}
        }

        public static ObservableCollection<User> GetContacts(User me)
        {
            ObservableCollection<User> contactList = new ObservableCollection<User>();
            for (int i = 0; i < Users.Count; i++)
            {
                if (me != null && me.Name != Users[i].Name) //me.Name != null &&
                {
                    contactList.Add(Users[i]);
                }
            }
            return contactList;
        }

        public static ObservableCollection<Message> GetMessageList(User me, User contact)
        {
            ObservableCollection<Message> messages = new ObservableCollection<Message>();
            
            for(int i = 0; i < Users.Count; i++)
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

        public static ObservableCollection<Message> GetGroupMessageList(User me)
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

        public static void SendMessage(User sender, User receiver, string text)
        {
            for (int i = 0; i < Users.Count; i++)
            {
                if (receiver.Name == Users[i].Name || sender.Name == Users[i].Name)
                {
                    Users[i].MessageList.Add(new Message(sender, receiver, text));
                }
            }
        }

        public static void SendGroupMessage(User sender, string text)
        {
            for (int i = 0; i < Users.Count; i++)
            {
                Users[i].MessageList.Add(new Message(sender, Users[i], text, true));
            }
        }

    }
}
