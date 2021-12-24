using Messenger.Common;
using System.Collections.Generic;


namespace Messenger.Network.Responses
{
    class GetUserListResponce
    {
        public string Result { get; set; }
        public List<User> UserList { get; set; }

        public GetUserListResponce(string result, List<User> userList)
        {
            Result = result;
            UserList = userList;
        }

        //public GetUserListResponce(string result)
        //{
        //    Result = result;
        //    UserList = new List<User>();
        //}

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(GetUserListResponce),
                Payload = this
            };

            return container;
        }
    }
}
