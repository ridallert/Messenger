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
        private bool _isPublicChatActive;
        private ObservableCollection<User> _contactList;
        private ObservableCollection<Message> _messageList;
        private string _login;
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
        public bool IsPublicChatActive
        {
            get { return _isPublicChatActive; }
            set
            {
                SetProperty(ref _isPublicChatActive, value);
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
        public string Login
        {
            get { return _login; }
            set
            {
                if (value != _login)
                {
                    SetProperty(ref _login, value);
                    SendMessageCommand.RaiseCanExecuteChanged();
                    StartPublicChatCommand.RaiseCanExecuteChanged();
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
                    SelectedUser.NewMessageCounter = null;
                    if (_login != null && value != null)
                    {
                        MessageList = _clientState.GetPrivateMessageList(value.Name);
                        IsPublicChatActive = false;
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
            _clientState.ContactListChanged += OnUserListChanged;
            _clientState.UserLoggedOut += OnUserLoggedOut;
            _clientState.NewMessageAdded += OnNewMessageAdded;
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
                    _webSocketClient.SendPrivateMessage(Login, SelectedUser.Name, NewMessage, DateTime.Now);
                    
                }
                if (_isPublicChatActive == true)
                {
                    //_clientState.SendGroupMessage(Me, NewMessage);
                    //MessageList = _clientState.GetGroupMessageList(Me);
                }
                
                NewMessage = null;
            }
        }
        private bool SendMessageCanExecute()
        {

            if (_isPublicChatActive == false)
            {
                return  Login != null &&
                        SelectedUser != null &&
                        ContactList != null &&
                        ContactList.Count != 0 && 
                        NewMessage != null &&
                        NewMessage != "";
            }
            else
            {
                return  Login != null &&
                        ContactList != null &&
                        ContactList.Count != 0 &&
                        NewMessage != null &&
                        NewMessage != "";
            }
        }


        private DelegateCommand _startPublicChatCommand;
        public DelegateCommand StartPublicChatCommand => _startPublicChatCommand ?? (_startPublicChatCommand = new DelegateCommand(StartPublicChatExecute, StartPublicChatCanExecute));

        public void StartPublicChatExecute()
        {
            IsPublicChatActive = true;
            SelectedUser = null;
            //MessageList = _clientState.GetPublicMessageList();
        }

        public bool StartPublicChatCanExecute()
        {
            return Login != null;
        }
        private void OnUserAuthorized()
        {
            Login = _clientState.Login;
            ContactList = _clientState.Contacts;
        }

        private void OnUserLoggedOut()
        {
            Login = null;
            ContactList = null;
            MessageList = null;
        }

        private void OnUserListChanged()
        {
            ContactList = _clientState.Contacts;
        }

        private void OnNewMessageAdded(Message message)
        {
            if (IsPublicChatActive)
            {
                //MessageList = _clientState.GetGroupMessageList(Login);
            }
            else
            {
                foreach (User contact in ContactList)
                {
                    if (contact.Name == message.Sender || contact.Name == message.Receiver)
                    {
                        contact.MessageList = _clientState.GetPrivateMessageList(contact.Name);
                        if (message.Sender == contact.Name && (SelectedUser == null || message.Sender != SelectedUser.Name))
                        {
                            contact.NewMessageCounter = contact.NewMessageCounter.HasValue ? contact.NewMessageCounter += 1 : contact.NewMessageCounter = 1;
                        }
                    }
                }
            }
        }

    }
}
