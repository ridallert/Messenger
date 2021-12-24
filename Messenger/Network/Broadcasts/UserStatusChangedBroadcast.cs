
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Network.Broadcasts
{
    class UserStatusChangedBroadcast
    {
        public string Name { get; set; }
        public OnlineStatus Status { get; set; }

        public UserStatusChangedBroadcast(string name, OnlineStatus status)
        {
            Name = name;
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
