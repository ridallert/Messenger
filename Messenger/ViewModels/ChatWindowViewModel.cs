using Messenger.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.ViewModels 
{
    public class ChatWindowViewModel : BindableBase
    {
        private ObservableCollection<User> _users;
        private User _me;
        private User _selectedUser;
        private string _newMessage;

        public ObservableCollection<User> Users
        {
            get { return _users; }
            set { _users = value; }
        }
        public User Me
        {
            get { return _me; }
            set
            {
                if (value != _me)
                {
                    SetProperty<User>(ref _me, value);
                }
            }
        }
        public User SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                if (value != _selectedUser)
                {
                    SetProperty<User>(ref _selectedUser, value);
                }
            }
        }
        public string NewMessage
        {
            get { return _newMessage; }
            set
            {
                if (value != _newMessage)
                {
                    SetProperty<string>(ref _newMessage, value);
                }
            }
        }

        public ChatWindowViewModel()
        {
            Me = new User("Ridal", OnlineStatus.Online);
            Users = Server.GetUsersList();

            //Users = new ObservableCollection<User>
            //{
            //    new User ("User A", OnlineStatus.Online),
            //    new User ("User B", OnlineStatus.Online),
            //    new User ("User C", OnlineStatus.Online),
            //    new User ("User D", OnlineStatus.Online),
            //};

            SelectedUser = Users.First();

            for (int i = 0; i < Users.Count; i++)
            {
                Users[i].MessageList = new ObservableCollection<Message>
                {
                    new Message(Users[i], Me, Me.Name + "Привет! Это " + Users[i].Name, DateTime.Now),
                    new Message(Users[i], Me, "Еще раз привет!", DateTime.Now),
                    new Message(Users[i], Me, "Пока", DateTime.Now)
                };
            }
        }

        private DelegateCommand<object> _addMessageCommand;
        public DelegateCommand<object> AddMessageCommand => _addMessageCommand ?? (_addMessageCommand = new DelegateCommand<object>(AddMessageExecute));

        private void AddMessageExecute(object obj)
        {
            string mes = obj as string;
            if (mes != null)
            {
                SelectedUser.MessageList.Add(new Message(Me, SelectedUser, mes, DateTime.Now));
            }
        }

    }
}
