using System;
using System.Collections.ObjectModel;

namespace Messenger.Models
{
    public interface IState
    {
        User AuthorizedUser { get; set; }
        ObservableCollection<User> Users { get; set; }

        event Action UserAuthorized;
        event Action UserListChanged;
        event Action UserLoggedOut;

        ObservableCollection<User> GetContacts(User me);
        ObservableCollection<Message> GetGroupMessageList(User me);
        ObservableCollection<Message> GetMessageList(User me, User contact);
        void SendGroupMessage(User sender, string text);
        void SendMessage(User sender, User receiver, string text);
    }
}