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
                SetProperty(ref _contactList, value);
            }
        }
        public ObservableCollection<Message> MessageList
        {
            get { return _messageList; }
            set
            {
                //_contactList = value;
                SetProperty(ref _messageList, value);
            }
        }
        public User Me
        {
            get { return _me; }
            set
            {
                if (value != _me)
                {
                    SetProperty(ref _me, value);
                    SendMessageCommand.RaiseCanExecuteChanged();
                    StartGroopChatCommand.RaiseCanExecuteChanged();
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
                    SetProperty(ref _selectedUser, value);
                    NewMessage = null;
                    SendMessageCommand.RaiseCanExecuteChanged();

                    if (Me != null && value != null)
                    {
                        MessageList = State.GetMessageList(Me, value);
                        IsGropChatActive = false;
                    }
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
                    SetProperty(ref _newMessage, value);
                    SendMessageCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public ChatWindowViewModel()
        {

            State.UserAuthorized += OnUserAuthorized;
            State.UserListChanged += OnUserListChanged;
            State.UserLoggedOut += OnUserLoggedOut;
        }

        private DelegateCommand _sendMessageCommand;
        public DelegateCommand SendMessageCommand => _sendMessageCommand ?? (_sendMessageCommand = new DelegateCommand(SendMessageExecute, SendMessageCanExecute));

        private void SendMessageExecute()
        {
            if (NewMessage != null)
            {
                if (SelectedUser != null)
                {
                    State.SendMessage(Me, SelectedUser, NewMessage);
                    MessageList = State.GetMessageList(Me, SelectedUser);

                }
                if (_isGroopChatActive)
                {
                    State.SendGroupMessage(Me, NewMessage);
                    MessageList = State.GetGroupMessageList(Me);
                }
                
                NewMessage = null;
            }
        }
        private bool SendMessageCanExecute()
        {
            if (_isGroopChatActive == false)
            {
                return  Me != null &&
                        SelectedUser != null &&
                        ContactList.Count != 0 &&
                        NewMessage != null &&
                        NewMessage != "";
            }
            else
            {
                return  Me != null &&
                        ContactList.Count != 0 &&
                        NewMessage != null &&
                        NewMessage != "";
            }
        }


        private DelegateCommand _startGroopChatCommand;
        public DelegateCommand StartGroopChatCommand => _startGroopChatCommand ?? (_startGroopChatCommand = new DelegateCommand(StartGroopChatExecute, StartGroopChatCanExecute));

        public void StartGroopChatExecute()
        {
            IsGropChatActive = true;
            SelectedUser = null;
            MessageList = State.GetGroupMessageList(Me);

        }

        public bool StartGroopChatCanExecute()
        {
            return Me != null;
        }
        private void OnUserAuthorized()
        {
            Me = State.AuthorizedUser;
            ContactList = State.GetContacts(Me);
        }

        private void OnUserLoggedOut()
        {
            for (int i = 0; i < State.Users.Count; i++)
            {
                if (Me.Name == State.Users[i].Name)
                {
                    State.Users[i].IsOnline = OnlineStatus.Offline;
                }
            }
            Me = null;
            ContactList = null;
            MessageList = null;
        }

        private void OnUserListChanged()
        {
            ContactList = State.GetContacts(Me);
        }

        //private void OnMessageListChanged()
        //{
        //    if (IsGropChatActive)
        //    {
        //        MessageList = State.GetGroupMessageList(Me);
        //    }
        //    else
        //    {
        //        MessageList = State.GetMessageList(Me, SelectedUser);
        //    }
        //}


    }
}
