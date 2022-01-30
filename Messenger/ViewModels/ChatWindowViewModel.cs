using Messenger.Common;
using Messenger.Models;
using Messenger.Network;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
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
        private IDialogService _dialogService;
        private WebSocketClient _webSocketClient;
        private ClientStateManager _clientState;

        private string _login;
        private ObservableCollection<ChatPresenter> _chatList;
        private Chat _selectedChat;
        private ObservableCollection<Message> _messageList;
        private string _newMessage;
        private int _caretPosition;

        public string Login
        {
            get { return _login; }
            set
            {
                if (value != _login)
                {
                    SetProperty(ref _login, value);
                    SendMessageCommand.RaiseCanExecuteChanged();
                    StartNewChatCommand.RaiseCanExecuteChanged();
                }
            }
        }
        public ObservableCollection<ChatPresenter> ChatList
        {
            get { return _chatList; }
            set { SetProperty(ref _chatList, value); }
        }

        public Chat SelectedChat
        {
            get { return _selectedChat; }
            set
            {
                if (value != _selectedChat)
                {
                    SetProperty(ref _selectedChat, value);
                    NewMessage = null;
                    //SendMessageCommand.RaiseCanExecuteChanged();

                    if (_login != null && value != null)
                    {
                        //_selectedChat.NewMessageCounter = null;
                        MessageList = _clientState.GetMessageList(value.ChatId);
                    }
                }
            }
        }
        public ObservableCollection<Message> MessageList
        {
            get { return _messageList; }
            set { SetProperty(ref _messageList, value); }
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
        
        
        public int CaretPosition
        {
            get { return _caretPosition; }
            set { SetProperty(ref _caretPosition, value); }
        }
        //private ObservableCollection<User> _contactList;
        //private ObservableCollection<User> _listViewSelectedItems;
        //private bool _isPublicChatActive;
        //private string _startNewChatButtonName;
        //private int _publicNewMessageCounter;
        //private bool _isPopupOpen;

        //public bool IsPopupOpen
        //{
        //    get { return _isPopupOpen; }
        //    set { SetProperty(ref _isPopupOpen, value); }
        //}
        //public string StartNewChatButtonName
        //{
        //    get { return _startNewChatButtonName; }
        //    set { SetProperty(ref _startNewChatButtonName, value); }
        //}

        //public bool IsPublicChatActive
        //{
        //    get { return _isPublicChatActive; }
        //    set
        //    {
        //        SetProperty(ref _isPublicChatActive, value);

        //        if (value == true)
        //        {
        //            _publicNewMessageCounter = 0;
        //            PublicChatButtonName = "Public chat";
        //        }
        //    }
        //}
        //public ObservableCollection<User> ContactList
        //{
        //    get { return _contactList; }
        //    set { SetProperty(ref _contactList, value); }
        //}
        //public ObservableCollection<User> ListViewSelectedItems
        //{
        //    get { return _listViewSelectedItems; }
        //    set { SetProperty(ref _listViewSelectedItems, value); }
        //}
        
        //public Contact SelectedContact
        //{
        //    get { return _selectedContact; }
        //    set
        //    {
        //        if (value != _selectedContact)
        //        {
        //            SetProperty(ref _selectedContact, value);
        //            NewMessage = null;
        //            SendMessageCommand.RaiseCanExecuteChanged();

        //            if (_login != null && value != null)
        //            {
        //                SelectedContact.NewMessageCounter = null;

        //                //MessageList = _clientState.GetPrivateMessageList(value.Title);
        //                //IsPublicChatActive = false;
        //            }
        //        }
        //    }
        //}
        

        public ChatWindowViewModel(IDialogService dialogService, ClientStateManager state, WebSocketClient webSocketClient)
        {
            _dialogService = dialogService;
            _webSocketClient = webSocketClient;
            _clientState = state;
            _clientState.UserAuthorized += OnUserAuthorized;
            _clientState.UserLoggedOut += OnUserLoggedOut;
            //_clientState.ContactListChanged += OnUserListChanged;
            _clientState.ChatListChanged += OnChatListChanged;

            //_clientState.NewMessageAdded += OnNewMessageAdded;

            //StartNewChatButtonName = "Start new chat";
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
                if (SelectedChat != null)
                {
                    //_webSocketClient.SendPrivateMessage(Login, SelectedChat.Title, NewMessage, DateTime.Now);

                }

                NewMessage = null;
            }
        }
        private bool SendMessageCanExecute()
        {
            return Login != null &&
                       ChatList != null &&
                       ChatList.Count != 0 &&
                       NewMessage != null &&
                       NewMessage != "" &&
                       SelectedChat != null;
        }

        //System.Windows.Controls.SelectedItemCollection
        private DelegateCommand<IList<object>> _startNewChatCommand;
        public DelegateCommand<IList<object>> StartNewChatCommand => _startNewChatCommand ?? (_startNewChatCommand = new DelegateCommand<IList<object>>(StartNewChatExecute, StartNewChatCanExecute));

        public void StartNewChatExecute(IList<object> selectedItems)
        {
            //List<User> selectedUsers = new List<User>();
            //foreach (object item in selectedItems)
            //{
            //    selectedUsers.Add((User)item);
            //}
            ////string sdf = ((User)selectedUsers.ToList()[0]).Name;
            //if (selectedUsers.Count != 0)
            //{
                
            //}
            //if (StartNewChatButtonName == "Create")
            //{
                
            //    StartNewChatButtonName = "Start new chat";
                
            //}
            //else
            //{
            //    StartNewChatButtonName = "Create";
            //}
            
            _dialogService.ShowDialog("NewChatDialog");
        }

        public bool StartNewChatCanExecute(IList<object> selectedItems)
        {
            return Login != null;
        }
        private void OnUserAuthorized()
        {
            Login = _clientState.Login;
            //ContactList = _clientState.GetContactList();
        }

        private void OnUserLoggedOut()
        {
            Login = null;
            //ContactList = null;
            MessageList = null;
            //IsPublicChatActive = false;
        }

        //private void OnUserListChanged()
        //{
        //    ContactList = _clientState.GetContactList();
        //}
        private void OnChatListChanged()
        {
            //foreach (IChat chat in _clientState.GetChatList())
            //{
            //    ChatList.Add(chat.ToChatPresenter(chat, Login));
            //}
            ChatList = _clientState.GetChatList();
        }
        //private void OnNewMessageAdded(Message message)
        //{
        //    if (message.Receiver == "Public chat")
        //    {
        //        //if (IsPublicChatActive)
        //        //{
        //        //    MessageList = _clientState.GetPublicMessageList();
        //        //}
        //        //else
        //        //{
        //        //    _publicNewMessageCounter++;
        //        //    PublicChatButtonName = "Public chat +" + _publicNewMessageCounter;
        //        //}
        //    }
        //    else
        //    {
        //        foreach (Contact contact in ContactList)
        //        {
        //            if (contact.Title == message.Sender || contact.Title == message.Receiver)
        //            {
        //                contact.MessageList = _clientState.GetPrivateMessageList(contact.Title);
        //                if (message.Sender == contact.Title && (SelectedContact == null || message.Sender != SelectedContact.Title))
        //                {
        //                    contact.NewMessageCounter = contact.NewMessageCounter.HasValue ? contact.NewMessageCounter += 1 : contact.NewMessageCounter = 1;
        //                }
        //            }
        //        }
        //    }
        //}
    }
}
