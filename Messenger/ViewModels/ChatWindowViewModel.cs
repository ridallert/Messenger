using Messenger.Common;
using Messenger.Models;
using Messenger.Network;
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
        private WebSocketClient _webSocketClient;
        private ClientStateManager _clientState;
        private bool? _isGroopChatActive;
        private ObservableCollection<User> _contactList;
        private ObservableCollection<Message> _messageList;
        private User _me;
        private User _selectedUser;
        private string _newMessage;
        private int _caretPosition;

        public int CaretPosition
        {
            get { return _caretPosition; }
            set
            {
                SetProperty(ref _caretPosition, value);
            }
        }
        public bool? IsGropChatActive
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
                        MessageList = _clientState.GetMessageList(Me, value);
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

        public ChatWindowViewModel(ClientStateManager state, WebSocketClient webSocketClient)
        {
            _webSocketClient = webSocketClient;
            _clientState = state;
            _clientState.UserAuthorized += OnUserAuthorized;
            _clientState.UserListChanged += OnUserListChanged;
            _clientState.UserLoggedOut += OnUserLoggedOut;
        }

        private DelegateCommand _newLineCommand;
        public DelegateCommand NewLineCommand => _newLineCommand ?? (_newLineCommand = new DelegateCommand(NewLineExecute, NewLineCanExecute));

        private void NewLineExecute()
        {
            if (NewMessage != null)
            {
                int temp = CaretPosition;
                NewMessage = NewMessage.Insert(CaretPosition, "\n");
                CaretPosition = temp + 1;
            }
        }

        private bool NewLineCanExecute()
        {
            return true;
        }

        private DelegateCommand _sendMessageCommand;
        public DelegateCommand SendMessageCommand => _sendMessageCommand ?? (_sendMessageCommand = new DelegateCommand(SendMessageExecute, SendMessageCanExecute));

        private void SendMessageExecute()
        {
            if (NewMessage != null)
            {
                if (SelectedUser != null)
                {
                    //_clientState.SendMessage(Me, SelectedUser, NewMessage);
                    //MessageList = _clientState.GetMessageList(Me, SelectedUser);
                    _webSocketClient.SendPrivateMessage(Me, SelectedUser, NewMessage, DateTime.Now);
                    
                }
                if (_isGroopChatActive == true)
                {
                    //_clientState.SendGroupMessage(Me, NewMessage);
                    //MessageList = _clientState.GetGroupMessageList(Me);
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
                        ContactList != null &&
                        ContactList.Count != 0 && 
                        NewMessage != null &&
                        NewMessage != "";
            }
            else
            {
                return  Me != null &&
                        ContactList != null &&
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
            MessageList = _clientState.GetGroupMessageList(Me);
        }

        public bool StartGroopChatCanExecute()
        {
            return Me != null;
        }
        private void OnUserAuthorized()
        {
            Me = _clientState.AuthorizedUser;
            ContactList = _clientState.GetContacts(Me);
        }

        private void OnUserLoggedOut()
        {
            for (int i = 0; i < _clientState.Users.Count; i++)
            {
                if (Me.Name == _clientState.Users[i].Name)
                {
                    _clientState.Users[i].IsOnline = OnlineStatus.Offline;
                }
            }
            Me = null;
            ContactList = null;
            MessageList = null;
        }

        private void OnUserListChanged()
        {
            ContactList = _clientState.GetContacts(Me);
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
