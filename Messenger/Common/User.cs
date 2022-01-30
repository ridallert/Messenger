using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Common
{
    public class User
    {
        private static int _idCounter;
        public int UserId { get; set; }
        public string Name { get; set; }
        public OnlineStatus IsOnline { get; set; }

        [JsonConstructor]
        public User(int userId, string name, OnlineStatus isOnline) : this(name, isOnline)
        {
            UserId = userId;
        }
        public User(string name, OnlineStatus isOnline) : this()
        {
            Name = name;
            IsOnline = isOnline;
        }
        public User()
        {
            UserId = _idCounter++;
        }
    }
}
