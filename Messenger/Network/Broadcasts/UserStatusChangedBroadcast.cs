
using Messenger.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Network.Broadcasts
{
    public class UserStatusChangedBroadcast
    {
        public string Name { get; set; }
        public int UserId { get; set; }
        public OnlineStatus Status { get; set; }

        public UserStatusChangedBroadcast(string name, int userId, OnlineStatus status)
        {
            Name = name;
            UserId = userId;
            Status = status;
        }

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(UserStatusChangedBroadcast),
                Payload = this
            };

            return container;
        }
    }
}
