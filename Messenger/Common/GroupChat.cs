using Messenger.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Common
{
    public class GroupChat: IChat
    {
        public string Title { get; set; }

        public GroupChat(User user)
        {
            Title = user.Name;
            Users = new List<User>();
            Users.Add(user);
        }
        public GroupChat(User user, OnlineStatus isOnline)
        {
            Title = user.Name;
            Users = new List<User>();
            Users.Add(user);
        }
        public GroupChat(string title)
        {
            Title = title;
            Users = new List<User>();
        }
        public GroupChat()
        {
            Users = new List<User>();
        }

        //public Contact(int contactId, string user, OnlineStatus isOnline)
        //{
        //    ContactId = contactId;
        //    Title = user;
        //    Users = new List<string>();
        //    Users.Add(user);
        //    IsOnline = isOnline;
        //}

        //[JsonConstructor]
        //public Contact(int contactId, string title, List<string> users)
        //{
        //    ContactId = contactId;
        //    Title = title;
        //    Users = users;
        //}
    }
}
