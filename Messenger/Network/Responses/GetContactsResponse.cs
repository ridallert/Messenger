using Messenger.Common;
using System.Collections.Generic;


namespace Messenger.Network.Responses
{
    public class GetContactsResponse
    {
        public string Result { get; set; }
        public List<User> UserList { get; set; }

        public GetContactsResponse(string result, List<User> userList)
        {
            Result = result;
            UserList = userList;
        }

        //public GetContactsResponse(string result)
        //{
        //    Result = result;
        //    UserList = new List<User>();
        //}

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(GetContactsResponse),
                Payload = this
            };

            return container;
        }
    }
}
