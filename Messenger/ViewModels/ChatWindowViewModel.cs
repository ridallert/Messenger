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
            Me = State.AuthorizedUser;
            Users = State.Users;

            //SelectedUser = Users.First();
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
