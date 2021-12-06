using Messenger.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Messenger.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private ObservableCollection<User> users;
        private User me = new User("Ridal", OnlineStatus.Online);
        private User selectedUser;
        private string newMessage;

        //private RelayCommand addUserCommand;
        //private RelayCommand addMessageCommand;

        public ObservableCollection<User> Users { get; set; }

        public User Me
        {
            get { return me; }
            set
            {
                if (value != me)
                {
                    SetProperty<User>(ref me, value);
                }
            }
        }

        public User SelectedUser
        {
            get { return selectedUser; }
            set
            {
                if (value != selectedUser)
                {
                    SetProperty<User>(ref selectedUser, value);
                }
            }
        }

        public string NewMessage
        {
            get { return newMessage; }
            set
            {
                if (value != newMessage)
                {
                    SetProperty<string>(ref newMessage, value);
                }
            }
        }

        public MainWindowViewModel()
        {
            //Me = new User("Ridal", OnlineStatus.Online);

            Users = new ObservableCollection<User>
            {
                new User ("User A", OnlineStatus.Online),
                new User ("User B", OnlineStatus.Online),
                new User ("User C", OnlineStatus.Online),
                new User ("User D", OnlineStatus.Online),
            };

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

        private DelegateCommand<object> addMessageCommand;
        public DelegateCommand<object> AddMessageCommand => addMessageCommand ?? (addMessageCommand = new DelegateCommand<object>(CommandLoadExecute));

        private void CommandLoadExecute(object obj)
        {
            string mes = obj as string;
            if (mes != null)
            {
                SelectedUser.MessageList.Add(new Message(Me, SelectedUser, mes, DateTime.Now));
            }
        }








        private string _title = "Prism Application";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

    }
}
