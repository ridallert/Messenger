﻿using Messenger.Common;
using System.Collections.Generic;


namespace Messenger.Network.Responses
{
    public class GetContactsResponse
    {
        public string Result { get; set; }
        public List<User> ContactList { get; set; }

        public GetContactsResponse(string result, List<User> contactList)
        {
            Result = result;
            ContactList = contactList;
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
