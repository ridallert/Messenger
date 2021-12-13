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
        private IState _serverState;
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
                SetProperty(ref _isGroopChatActive, value);
            }
        }
        public ObservableCollection<User> ContactList
        {
            get { return _contactList; }
            set
            {
                SetProperty(ref _contactList, value);
            }
        }
        public ObservableCollection<Message> MessageList
        {
            get { return _messageList; }
            set
            {
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
                        MessageList = _serverState.GetMessageList(Me, value);
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

        public ChatWindowViewModel(IState state)
        {
            _serverState = state;
            _serverState.UserAuthorized += OnUserAuthorized;
            _serverState.UserListChanged += OnUserListChanged;
            _serverState.UserLoggedOut += OnUserLoggedOut;
        }

        private DelegateCommand _sendMessageCommand;
        public DelegateCommand SendMessageCommand => _sendMessageCommand ?? (_sendMessageCommand = new DelegateCommand(SendMessageExecute, SendMessageCanExecute));

        private void SendMessageExecute()
        {
            if (NewMessage != null)
            {
                if (SelectedUser != null)
                {
                    _serverState.SendMessage(Me, SelectedUser, NewMessage);
                    MessageList = _serverState.GetMessageList(Me, SelectedUser);

                }
                if (_isGroopChatActive)
                {
                    _serverState.SendGroupMessage(Me, NewMessage);
                    MessageList = _serverState.GetGroupMessageList(Me);
                }
                
                NewMessage = null;
            }
        }
        private bool SendMessageCanExecute()
        {

            if (_isGroopChatActive == false)
            {
                return Me != null &&
                        SelectedUser != null &&
                        SelectedUser.IsOnline == OnlineStatus.Online && //--------------
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
            MessageList = _serverState.GetGroupMessageList(Me);

        }

        public bool StartGroopChatCanExecute()
        {
            return Me != null;
        }
        private void OnUserAuthorized()
        {
            Me = _serverState.AuthorizedUser;
            ContactList = _serverState.GetContacts(Me);
        }

        private void OnUserLoggedOut()
        {
            for (int i = 0; i < _serverState.Users.Count; i++)
            {
                if (Me.Name == _serverState.Users[i].Name)
                {
                    _serverState.Users[i].IsOnline = OnlineStatus.Offline;
                }
            }
            Me = null;
            ContactList = null;
            MessageList = null;
        }

        private void OnUserListChanged()
        {
            ContactList = _serverState.GetContacts(Me);
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
