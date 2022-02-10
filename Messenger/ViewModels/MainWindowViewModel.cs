namespace Messenger.ViewModels
{
    using System.Windows;

    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Services.Dialogs;

    using Messenger.Models;
    using Messenger.Network;

    public class MainWindowViewModel : BindableBase
    {
        #region Fields

        private readonly WebSocketClient _webSocketClient;
        private readonly ClientStateManager _clientState;
        private readonly IDialogService _dialogService;
        private string _title;
        public string _loginButtonContent;
        private DelegateCommand _showAuthDialCommand;
        private DelegateCommand _showLogWindowCommand;

        #endregion //Fields

        #region Properties

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
       
        public string LoginButtonContent
        {
            get { return _loginButtonContent; }
            set
            {
                SetProperty(ref _loginButtonContent, value);
            }
        }
        
        public DelegateCommand ShowAuthorizationDialogCommand => _showAuthDialCommand ??
            (_showAuthDialCommand = new DelegateCommand(ShowAuthDialogExecute));
        
        public DelegateCommand ShowLogWindowDialogCommand => _showLogWindowCommand ??
            (_showLogWindowCommand = new DelegateCommand(ShowLogWindowExecute, ShowLogWindowCanExecute));

        #endregion //Properties

        #region Constructors

        public MainWindowViewModel(IDialogService dialogService, ClientStateManager state, WebSocketClient webSocketClient)
        {
            _webSocketClient = webSocketClient;
            _clientState = state;
            _dialogService = dialogService;
            
            Title = "KeepTalk";
            LoginButtonContent = "Log in";

            _webSocketClient.Disconnected += OnDisconnected;
            _clientState.UserAuthorized += OnUserAuthorized;
            _clientState.UserLoggedOut += OnUserLoggedOut;
        }

        #endregion //Constructors

        #region Methods

        private void OnDisconnected()
        {
            _clientState.LogOut();
            Application.Current.Dispatcher.InvokeAsync(() => ShowNotificationWindow("Server is not available"));
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

        private void OnUserAuthorized()
        {
            LoginButtonContent = "Log out";
            ShowLogWindowDialogCommand.RaiseCanExecuteChanged();
        }

        private void OnUserLoggedOut()
        {
            LoginButtonContent = "Log in";
            ShowLogWindowDialogCommand.RaiseCanExecuteChanged();
        }

        private void ShowAuthDialogExecute()
        {
            if (_clientState.Login == null)
            {
                _dialogService.ShowDialog("AuthorizationDialog");
            }
            else
            {
                _clientState.LogOut();
                _webSocketClient.Disconnect();
            }
        }

        private void ShowLogWindowExecute()
        {
            _dialogService.ShowDialog("LogWindow");
        }

        private bool ShowLogWindowCanExecute()
        {
            return _clientState.Login != null;
        }

        #endregion //Methods
    }
}
