namespace Messenger.Common
{
    using Prism.Mvvm;
    using System.Collections.Generic;
    public class Chat: BindableBase
    {
        //private static int _idCounter;
        private string _title;
        private List<User> _users;
        private List<Message> _messages;
        public int ChatId { get; set; }
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        public List<User> Users
        {
            get { return _users; }
            set { SetProperty(ref _users, value); }
        }
        public List<Message> Messages
        {
            get { return _messages; }
            set { SetProperty(ref _messages, value); }
        }

        public Chat(string title, List<User> users) : this(title)
        {
            Users.AddRange(users);
        }
        public Chat(string title) : this()
        {
            Title = title;
        }
        public Chat(User userA, User userB) : this()
        {
            Users.Add(userA);
            Users.Add(userB);
        }
        public Chat()
        {
            //ChatId = _idCounter++;
            Users = new List<User>();
            Messages = new List<Message>();
        }
        public ChatPresenter ToChatPresenter(string login)
        {
            ChatPresenter presenter = new ChatPresenter();
            presenter.ChatId = ChatId;
            presenter.Messages = Messages;

            if (Title!=null)
            {
                presenter.Title = Title;
            }
            else
            {
                User targetUser = Users.Find(user => user.Name != login);
                presenter.Title = targetUser.Name;
                presenter.IsOnline = targetUser.IsOnline;
            }
            presenter.Users = Users;
            return presenter;
        }
    }
}
