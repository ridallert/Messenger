using Messenger.Models;
using Messenger.Network;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Messenger.ViewModels
{
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
            _clientState = state;
            _dialogService = dialogService;

            Title = "PrismMessenger";
            LoginButtonContent = "Log in";

            _clientState.UserAuthorized += OnUserAuthorized;
            _clientState.UserLoggedOut += OnUserLoggedOut;
        }

        private void OnUserAuthorized()
        {
            LoginButtonContent = "Log out";
        }

        private void OnUserLoggedOut()
        {
            LoginButtonContent = "Log in";
            _webSocketClient.Disconnect();
        }


        private DelegateCommand _showAuthDialCommand;
        public DelegateCommand ShowAuthorizationDialogCommand => _showAuthDialCommand ?? (_showAuthDialCommand = new DelegateCommand(ShowAuthDialogExecute));

        private void ShowAuthDialogExecute()
        {
            if (_clientState.AuthorizedUser == null)
            {
                _dialogService.ShowDialog("AuthorizationDialog");
            }
            else
            {
                _clientState.AuthorizedUser = null;
            }
        }
    }
}
