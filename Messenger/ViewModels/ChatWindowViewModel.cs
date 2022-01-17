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

        private ObservableCollection<Message> _messageList;
        private ObservableCollection<Contact> _contactList;
        private Contact _selectedContact;

        private string _login;
        private string _newMessage;
        private bool _isPublicChatActive;
        private int _caretPosition;

        private string _publicChatButtonName;
        private int _publicNewMessageCounter;

        public string PublicChatButtonName
        {
            get { return _publicChatButtonName; }
            set { SetProperty(ref _publicChatButtonName, value); }
        }
        public int CaretPosition
        {
            get { return _caretPosition; }
            set { SetProperty(ref _caretPosition, value); }
        }
        public bool IsPublicChatActive
        {
            get { return _isPublicChatActive; }
            set
            {
                SetProperty(ref _isPublicChatActive, value);

                if (value == true)
                {
                    _publicNewMessageCounter = 0;
                    PublicChatButtonName = "Public chat";
                }
            }
        }
        public ObservableCollection<Contact> ContactList
        {
            get { return _contactList; }
            set { SetProperty(ref _contactList, value); }
        }
        public ObservableCollection<Message> MessageList
        {
            get { return _messageList; }
            set { SetProperty(ref _messageList, value); }
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
        public Contact SelectedContact
        {
            get { return _selectedContact; }
            set
            {
                if (value != _selectedContact)
                {
                    SetProperty(ref _selectedContact, value);
                    NewMessage = null;
                    SendMessageCommand.RaiseCanExecuteChanged();

                    if (_login != null && value != null)
                    {
                        SelectedContact.NewMessageCounter = null;

                        MessageList = _clientState.GetPrivateMessageList(value.Title);
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
            _clientState.UserLoggedOut += OnUserLoggedOut;
            _clientState.ContactListChanged += OnUserListChanged;
            _clientState.NewMessageAdded += OnNewMessageAdded;

            PublicChatButtonName = "Public chat";
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
                if (SelectedContact != null)
                {
                    _webSocketClient.SendPrivateMessage(Login, SelectedContact.Title, NewMessage, DateTime.Now);

                }
                if (_isPublicChatActive == true)
                {
                    _webSocketClient.SendPublicMessage(Login, NewMessage, DateTime.Now);
                }

                NewMessage = null;
            }
        }
        private bool SendMessageCanExecute()
        {
            if (_isPublicChatActive == false)
            {
                return Login != null &&
                       ContactList != null &&
                       ContactList.Count != 0 &&
                       NewMessage != null &&
                       NewMessage != "" &&
                       SelectedContact != null;
            }
            else
            {
                return Login != null &&
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
            SelectedContact = null;
            MessageList = _clientState.GetPublicMessageList();
        }

        public bool StartPublicChatCanExecute()
        {
            return Login != null;
        }
        private void OnUserAuthorized()
        {
            Login = _clientState.Login;
            ContactList = _clientState.GetContactList();
        }

        private void OnUserLoggedOut()
        {
            Login = null;
            ContactList = null;
            MessageList = null;
            IsPublicChatActive = false;
        }

        private void OnUserListChanged()
        {
            ContactList = _clientState.GetContactList();
        }

        private void OnNewMessageAdded(Message message)
        {
            if (message.Receiver == "Public chat")
            {
                if (IsPublicChatActive)
                {
                    MessageList = _clientState.GetPublicMessageList();
                }
                else
                {
                    _publicNewMessageCounter++;
                    PublicChatButtonName = "Public chat +" + _publicNewMessageCounter;
                }
            }
            else
            {
                foreach (Contact contact in ContactList)
                {
                    if (contact.Title == message.Sender || contact.Title == message.Receiver)
                    {
                        contact.MessageList = _clientState.GetPrivateMessageList(contact.Title);
                        if (message.Sender == contact.Title && (SelectedContact == null || message.Sender != SelectedContact.Title))
                        {
                            contact.NewMessageCounter = contact.NewMessageCounter.HasValue ? contact.NewMessageCounter += 1 : contact.NewMessageCounter = 1;
                        }
                    }
                }
            }
        }
    }
}
