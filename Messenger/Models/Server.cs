using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Models
{
    public static class Server
    {
        public static ObservableCollection<User> GetUsersList()
        {
            ObservableCollection<User> users = new ObservableCollection<User>
            {
                new User ("User A", OnlineStatus.Online),
                new User ("User B", OnlineStatus.Online),
                new User ("User C", OnlineStatus.Online),
                new User ("User D", OnlineStatus.Online),
            };

            return users;
        }
    }
}
