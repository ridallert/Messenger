namespace Messenger.Common
{
    using System.Collections.Generic;
    public class Chat
    {
        private static int _idCounter;
        public int ChatId { get; set; }
        public string Title { get; set; }
        public List<User> Users { get; set; }
        public List<Message> Messages { get; set; }

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
            ChatId = _idCounter++;
            Users = new List<User>();
            Messages = new List<Message>();
        }
        public ChatPresenter ToChatPresenter(Chat chat, string login)
        {
            ChatPresenter presenter = new ChatPresenter();
            presenter.ChatId = chat.ChatId;
            presenter.Messages = chat.Messages;

            if (chat.Title!=null)
            {
                presenter.Title = chat.Title;
                presenter.Users = chat.Users;
            }
            else
            {
                User targetUser = chat.Users.Find(user => user.Name != login);
                presenter.Title = targetUser.Name;
                presenter.IsOnline = targetUser.IsOnline;
            }
            
            return presenter;
        }
    }
}
