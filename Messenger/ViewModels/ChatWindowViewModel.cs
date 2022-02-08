using Messenger.Common;
using Messenger.Models;
using Messenger.Network;
using Messenger.Network.Responses;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Messenger.ViewModels
{
    public class ChatWindowViewModel : BindableBase
    {
        private IDialogService _dialogService;
        private WebSocketClient _webSocketClient;
        private ClientStateManager _clientState;

        private string _login;
        private int? _userId;
        private ObservableCollection<ChatPresenter> _chatList;
        private ChatPresenter _selectedChat;
        private ObservableCollection<Message> _messageList;
        private string _newMessage;
        private int _caretPosition;

        private bool _isNewMessageEnabled;
        public bool IsNewMessageEnabled
        {
            get { return _isNewMessageEnabled; }
            set { SetProperty(ref _isNewMessageEnabled, value); }
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
                    StartNewChatCommand.RaiseCanExecuteChanged();
                }
            }
        }
        public ObservableCollection<ChatPresenter> ChatList
        {
            get { return _chatList; }
            set { SetProperty(ref _chatList, value); }
        }

        public ChatPresenter SelectedChat
        {
            get { return _selectedChat; }
            set
            {
                if (value != _selectedChat)
                {
                    SetProperty(ref _selectedChat, value);
                    NewMessage = null;
                    SendMessageCommand.RaiseCanExecuteChanged();

                    if (value != null)
                    {
                        SelectedChat.NewMessageCounter = null;
                        MessageList = new ObservableCollection<Message>(SelectedChat.Messages);
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

        public ChatWindowViewModel(IDialogService dialogService, ClientStateManager state, WebSocketClient webSocketClient)
        {
            _dialogService = dialogService;
            _webSocketClient = webSocketClient;
            _clientState = state;

            _clientState.UserAuthorized += OnUserAuthorized;
            _clientState.UserLoggedOut += OnUserLoggedOut;
            _clientState.UserStatusChanged += OnUserStatusChanged;
            _clientState.ChatListLoaded += OnChatListLoaded;
            _clientState.NewChatAdded += OnNewChatAdded;
            _clientState.MessageReceived += OnMessageReceived;
            _webSocketClient.SendMessageResponseCame += OnSendMessageResponseCame;
        }

        private void OnSendMessageResponseCame(SendMessageResponse response)
        {
            Application.Current.Dispatcher.InvokeAsync(() => ShowNotificationWindow(response.Result));
        }
        private void ShowNotificationWindow(string message)
        {
            var par = new DialogParameters
            {
                { "result", message }
            };

            _dialogService.Show("NotificationWindow", par, Callback);
        }
        void Callback(IDialogResult result) { }

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
                    _webSocketClient.SendMessage(_userId.Value, SelectedChat.ChatId, NewMessage, DateTime.Now);
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

        private DelegateCommand<IList<object>> _startNewChatCommand;
        public DelegateCommand<IList<object>> StartNewChatCommand => _startNewChatCommand ?? (_startNewChatCommand = new DelegateCommand<IList<object>>(StartNewChatExecute, StartNewChatCanExecute));

        public void StartNewChatExecute(IList<object> selectedItems)
        {
            _dialogService.ShowDialog("NewChatDialog");
        }

        public bool StartNewChatCanExecute(IList<object> selectedItems)
        {
            return Login != null;
        }

        private void OnUserStatusChanged(User arg)
        {
            foreach (ChatPresenter chat in ChatList)
            {
                User tempUser = chat.Users.Find(user => user.UserId == arg.UserId);

                if (tempUser != null)
                {
                    tempUser.IsOnline = arg.IsOnline;
                    if (chat.Users.Count == 2)
                    {
                        chat.IsOnline = arg.IsOnline;
                    }
                }
            }
        }
        private void OnUserAuthorized()
        {
            Login = _clientState.Login;
            _userId = _clientState.UserId;
            IsNewMessageEnabled = true;
        }

        private void OnUserLoggedOut()
        {
            Login = null;
            ChatList = null;
            MessageList = null;
            IsNewMessageEnabled = false;
        }
        private void OnChatListLoaded()
        {
            ChatList = _clientState.GetChatList();
        }
        private void OnNewChatAdded(ChatPresenter newChat)
        {
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                ChatList.Add(newChat);
            });
        }

        private void OnMessageReceived(Message message)
        {
            foreach (ChatPresenter chat in ChatList)
            {
                if (chat.ChatId == message.ChatId && (SelectedChat == null || message.ChatId != SelectedChat.ChatId))
                {
                    chat.NewMessageCounter = chat.NewMessageCounter.HasValue ? chat.NewMessageCounter += 1 : chat.NewMessageCounter = 1;
                }
            }
            if (SelectedChat != null && message.ChatId == SelectedChat.ChatId)
            {
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageList.Add(message);
                });
            }
        }
    }
}
