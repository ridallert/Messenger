﻿namespace Messenger.ViewModels
{
    using Messenger.Models;
    using Messenger.Network;
    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Services.Dialogs;
    using System.Windows;

    public class MainWindowViewModel : BindableBase
    {
        private WebSocketClient _webSocketClient;
        private readonly ClientStateManager _clientState;
        private IDialogService _dialogService;
        private string _title;
        public string _loginButtonContent;

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
        public MainWindowViewModel(IDialogService dialogService, ClientStateManager state, WebSocketClient webSocketClient)
        {
            _webSocketClient = webSocketClient;
            _webSocketClient.Disconnected += OnDisconnected;
            _clientState = state;
            _dialogService = dialogService;

            Title = "KeepTalk";
            LoginButtonContent = "Log in";

            _clientState.UserAuthorized += OnUserAuthorized;
            _clientState.UserLoggedOut += OnUserLoggedOut;
        }

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
        void Callback(IDialogResult result) {}
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

        private DelegateCommand _showAuthDialCommand;
        public DelegateCommand ShowAuthorizationDialogCommand => _showAuthDialCommand ?? (_showAuthDialCommand = new DelegateCommand(ShowAuthDialogExecute));

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

        private DelegateCommand _showLogWindowCommand;
        public DelegateCommand ShowLogWindowDialogCommand => _showLogWindowCommand ?? (_showLogWindowCommand = new DelegateCommand(ShowLogWindowExecute, ShowLogWindowCanExecute));

        private void ShowLogWindowExecute()
        {
            _dialogService.ShowDialog("LogWindow");
        }
        private bool ShowLogWindowCanExecute()
        {
            return _clientState.Login != null;
        }
    }
}
