using Messenger.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Messenger.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private IDialogService _dialogService;

        private string _title;
        //private Server _myServer;
        private ObservableCollection<User> _users;
        private User _me;
        private User _selectedUser;
        private string _newMessage;
        
        

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        //private Server MyServer
        //{
        //    get { return _myServer; }
        //    set { _myServer = value; }
        //}
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
        

        public MainWindowViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;

            Title = "Prism Application";
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
        public DelegateCommand<object> AddMessageCommand
        {   //public DelegateCommand<object> AddMessageCommand => _addMessageCommand ?? (_addMessageCommand = new DelegateCommand<object>(CommandLoadExecute));
            get
            {
                if (_addMessageCommand != null)
                {
                    return _addMessageCommand;
                }
                else
                {
                    _addMessageCommand = new DelegateCommand<object>(AddMessageExecute);
                    return _addMessageCommand;
                }
            }
        }
        private void AddMessageExecute(object obj)
        {
            string mes = obj as string;
            if (mes != null)
            {
                SelectedUser.MessageList.Add(new Message(Me, SelectedUser, mes, DateTime.Now));
            }
        }

        private DelegateCommand<object> _showAuthorizationWindowCommand;
        public DelegateCommand<object> ShowAuthorizationWindowCommand
        {   //public DelegateCommand<object> AddMessageCommand => _addMessageCommand ?? (_addMessageCommand = new DelegateCommand<object>(CommandLoadExecute));
            get
            {
                if (_showAuthorizationWindowCommand != null)
                {
                    return _showAuthorizationWindowCommand;
                }
                else
                {
                    _showAuthorizationWindowCommand = new DelegateCommand<object>(ShowAuthorizationWindowExecute);
                    return _showAuthorizationWindowCommand;
                }
            }
        }

        private void ShowAuthorizationWindowExecute(object obj)
        {
            var message = "This is a message that should be shown in the dialog.";
            //using the dialog service as-is
            _dialogService.ShowDialog("NotificationDialog", new DialogParameters($"message={message}"), r =>
            {
                if (r.Result == ButtonResult.None)
                    Title = "Result is None";
                else if (r.Result == ButtonResult.OK)
                    Title = "Result is OK";
                else if (r.Result == ButtonResult.Cancel)
                    Title = "Result is Cancel";
                else
                    Title = "I Don't know what you did!?";
            });
        }

    }
}
