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
            set { _users = value; }
        }
        static public User AuthorizedUser
        {
            get { return _authorizedUser; }
            set { _authorizedUser = value; }
        }

        static State()
        {
            AuthorizedUser = new User("Ridal", OnlineStatus.Online);

            Users = new ObservableCollection<User>
            {
                new User ("User A", OnlineStatus.Online),
                new User ("User B", OnlineStatus.Offline),
                new User ("User C", OnlineStatus.Online),
                new User ("User D", OnlineStatus.Offline),
            };

            for (int i = 0; i < Users.Count; i++)
            {
                Users[i].MessageList = new ObservableCollection<Message>
                {
                    new Message(Users[i], AuthorizedUser, AuthorizedUser.Name + ", Привет! Это " + Users[i].Name, DateTime.Now),
                    new Message(Users[i], AuthorizedUser, "Еще раз привет!", DateTime.Now),
                    new Message(Users[i], AuthorizedUser, "Пока", DateTime.Now)
                };
            }
        }
    }
}
