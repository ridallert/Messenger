namespace Messenger.ViewModels
{
    using System;
    using System.Windows;

    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Services.Dialogs;
    
    using Messenger.Network;
    using Messenger.Network.Responses;
    

    class AuthorizationDialogViewModel : BindableBase, IDialogAware
    {
        #region Fields

        private readonly WebSocketClient _webSocketClient;
        private readonly IDialogService _dialogService;
        private string _title;
        private string _serverConfigButtonName;
        private string _login;
        private DelegateCommand _authorizeUserCommand;
        private DelegateCommand _closeDialogCommand;
        private DelegateCommand _showServerConfigCommand;

        #endregion //Fields

        #region Properties

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public string ServerConfigButtonName
        {
            get { return _serverConfigButtonName; }
            set { SetProperty(ref _serverConfigButtonName, value); }
        }
        
        public string Login
        {
            get { return _login; }
            set
            {
                SetProperty(ref _login, value);
                AuthorizeUserCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand AuthorizeUserCommand => _authorizeUserCommand ??
            (_authorizeUserCommand = new DelegateCommand(AuthorizeUserExecute, AuthorizeUserCanExecute));
        
        public DelegateCommand CloseDialogCommand => _closeDialogCommand ??
            (_closeDialogCommand = new DelegateCommand(CloseDialog));

        public DelegateCommand ShowServerConfigCommand => _showServerConfigCommand ??
            (_showServerConfigCommand = new DelegateCommand(ShowServerConfigExecute));

        #endregion //Properties

        #region Events

        public event Action<IDialogResult> RequestClose;

        #endregion //Events

        #region Constructors

        public AuthorizationDialogViewModel(IDialogService dialogService, WebSocketClient webSocketClient)
        {
            _dialogService = dialogService;
            _webSocketClient = webSocketClient;

            _title = "Authorization";
            ServerConfigButtonName = "Server config (Disconnected)";
        }

        #endregion //Constructors

        #region Methods

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            _webSocketClient.Connected -= OnWebSocketConnected;
            _webSocketClient.Disconnected -= OnWebSocketDisconnected;
            _webSocketClient.AuthorizationResponseСame -= ShowAuthorizationResult;

            RequestClose?.Invoke(dialogResult);
        }

        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
            _webSocketClient.Connected += OnWebSocketConnected;
            _webSocketClient.Disconnected += OnWebSocketDisconnected;
            _webSocketClient.AuthorizationResponseСame += ShowAuthorizationResult;

            if (_webSocketClient.IsConnected == false)
            {
                _webSocketClient.Connect("127.0.0.1", 7890);
            }
        } 

        public virtual bool CanCloseDialog()
        {
            return true;
        }

        public virtual void OnDialogClosed()
        {
            //Необходим для реализации IDialogAware
        }

        protected virtual void CloseDialog()
        {
            ButtonResult result = ButtonResult.None;
            RaiseRequestClose(new DialogResult(result));
        }

        private void ShowAuthorizationResult(AuthorizationResponse response)
        {
            if (response.Result == "Success")
            {
                Application.Current.Dispatcher.InvokeAsync(CloseDialog);
                _webSocketClient.GetContacts(response.UserId);
                _webSocketClient.GetChatList(response.UserId);
                _webSocketClient.GetEventLog(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1));
            }

            Application.Current.Dispatcher.InvokeAsync(() => ShowNotificationWindow(response));
        }

        private void ShowNotificationWindow(AuthorizationResponse response)
        {
            var par = new DialogParameters
            {
                { "result", response.Result }
            };

            _dialogService.Show("NotificationWindow", par, Callback);
        }

        private void Callback(IDialogResult result)
        {
            //Необходим для вызова диалогового окна
        }

        private void OnWebSocketConnected()
        {
            ServerConfigButtonName = "Server config (Connected)";
            AuthorizeUserCommand.RaiseCanExecuteChanged();
        }

        private void OnWebSocketDisconnected()
        {
            ServerConfigButtonName = "Server config (Disconnected)";
            AuthorizeUserCommand.RaiseCanExecuteChanged();
        }

        private void AuthorizeUserExecute()
        {
            _webSocketClient.Authorize(Login);
        }

        private bool AuthorizeUserCanExecute()
        {
            if (Login != null && Login != "" && _webSocketClient.IsConnected == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ShowServerConfigExecute()
        {
            _dialogService.ShowDialog("ServerConfigDialog");
        }

        #endregion //Methods
    }
}
