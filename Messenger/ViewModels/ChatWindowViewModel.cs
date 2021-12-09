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
        private bool _isGroopChatActive;
        private ObservableCollection<User> _contactList;
        private ObservableCollection<Message> _messageList;
        private User _me;
        private User _selectedUser;
        private string _newMessage;

        public bool IsGropChatActive
        {
            get { return _isGroopChatActive; }
            set
            {
                SetProperty<bool>(ref _isGroopChatActive, value);
            }
        }
        public ObservableCollection<User> ContactList
        {
            get { return _contactList; }
            set
            {
                //_contactList = value;
                SetProperty<ObservableCollection<User>>(ref _contactList, value);
            }
        }
        public ObservableCollection<Message> MessageList
        {
            get { return _messageList; }
            set
            {
                //_contactList = value;
                SetProperty<ObservableCollection<Message>>(ref _messageList, value);
            }
        }
        public User Me
        {
            get { return _me; }
            set
            {
                if (value != _me)
                {
                    SetProperty<User>(ref _me, value);
                    SendMessageCommand.RaiseCanExecuteChanged();
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
                    NewMessage = null;
                    SendMessageCommand.RaiseCanExecuteChanged();
                    //MessageList = State.GetMessageList(Me, value);
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
                    SendMessageCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public ChatWindowViewModel()
        {

            State.UserAuthorized += OnUserAuthorized;
            State.UserListChanged += OnUserListChanged;
            User.MessageListChanged += OnMessageListChanged;
            //SelectedUser = Users.First();
        }

        private DelegateCommand _sendMessageCommand;
        public DelegateCommand SendMessageCommand => _sendMessageCommand ?? (_sendMessageCommand = new DelegateCommand(SendMessageExecute, SendMessageCanExecute));

        private void SendMessageExecute()
        {
            if (NewMessage != null && (SelectedUser != null))
            {
                if (SelectedUser != null)
                {
                    State.SendMessage(Me, SelectedUser, NewMessage);

                }
                if (_isGroopChatActive)
                {
                    State.SendGroupMessage(Me, NewMessage);
                }

                NewMessage = null;
            }
        }
        private bool SendMessageCanExecute()
        {
            return Me != null &&
                    SelectedUser != null &&
                    ContactList.Count != 0 &&
                    NewMessage != null &&
                    NewMessage != "";
        }

        private void OnUserAuthorized()
        {
            Me = State.AuthorizedUser;
            ContactList = State.GetContacts(Me);
        }
        private void OnUserListChanged()
        {
            ContactList = State.GetContacts(Me);
        }
        private void OnMessageListChanged()
        {
            if (IsGropChatActive)
            {
                MessageList = State.GetGroupMessageList(Me);
            }
            else
            {
                MessageList = State.GetMessageList(Me, SelectedUser);
            }
        }


    }
}
