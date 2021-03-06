namespace Messenger.DataObjects
{
    using System.Collections.Generic;

    using Prism.Mvvm;
    
    public class Chat: BindableBase
    {
        #region Fields

        private string _title;
        private List<User> _users;
        private List<Message> _messages;

        #endregion //Fields

        #region Properties

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

        #endregion //Properties

        #region Methods

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

        #endregion //Methods
    }
}
