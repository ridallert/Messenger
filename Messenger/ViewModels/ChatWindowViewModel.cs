namespace Messenger.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;

    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Services.Dialogs;

    using Messenger.DataObjects;
    using Messenger.Models;
    using Messenger.Network;
    using Messenger.Network.Responses;

    public class ChatWindowViewModel : BindableBase
    {
        #region Fields

        private readonly IDialogService _dialogService;
        private readonly WebSocketClient _webSocketClient;
        private readonly ClientStateManager _clientState;
        private string _login;
        private int? _userId;
        private ObservableCollection<ChatPresenter> _chatList;
        private ChatPresenter _selectedChat;
        private ObservableCollection<Message> _messageList;
        private string _newMessage;
        private int _caretPosition;
        private bool _isNewMessageEnabled;
        private DelegateCommand _newLineCommand;
        private DelegateCommand _sendMessageCommand;
        private DelegateCommand _startNewChatCommand; 

        #endregion //Fields

        #region Properties

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

        public DelegateCommand NewLineCommand => _newLineCommand ??
            (_newLineCommand = new DelegateCommand(NewLineExecute, NewLineCanExecute));

        public DelegateCommand SendMessageCommand => _sendMessageCommand ??
            (_sendMessageCommand = new DelegateCommand(SendMessageExecute, SendMessageCanExecute));

        public DelegateCommand StartNewChatCommand => _startNewChatCommand ??
            (_startNewChatCommand = new DelegateCommand(StartNewChatExecute, StartNewChatCanExecute));

        #endregion //Properties

        #region Constructors

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

        #endregion //Constructors

        #region Methods

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

        private void Callback(IDialogResult result)
        {
            //Необходим для вызова диалогового окна
        }

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

        private void StartNewChatExecute()
        {
            _dialogService.ShowDialog("NewChatDialog");
        }

        private bool StartNewChatCanExecute()
        {
            return Login != null;
        }

        private void OnUserStatusChanged(User arg)
        {
            foreach (ChatPresenter presenter in ChatList)
            {
                User tempUser = presenter.Users.Find(user => user.UserId == arg.UserId);

                if (tempUser != null)
                {
                    tempUser.IsOnline = arg.IsOnline;

                    if (presenter.Users.Count == 2 && presenter.Title != "Public chat")
                    {
                        presenter.IsOnline = arg.IsOnline;
                    }
                }
                else
                {
                    ChatList.FirstOrDefault(chat => chat.Title == "Pubclic chat")?.Users.Add(arg);
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

        #endregion //Methods
    }
}
