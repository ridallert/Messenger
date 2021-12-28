using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messenger.Models;
using Messenger.Network;
using Messenger.Common;
using Messenger.Network.Responses;

namespace Messenger.ViewModels
{
    class AuthorizationDialogViewModel : BindableBase, IDialogAware
    {
        private ClientState _clientState;
        private WebSocketClient _webSocketClient; //--------------
        private IDialogService _dialogService;


        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private string _login;
        public string Login
        {
            get { return _login; }
            set
            {
                SetProperty(ref _login, value);
                AuthorizeUserCommand.RaiseCanExecuteChanged();
            }
        }

        public AuthorizationDialogViewModel(IDialogService dialogService, ClientState state, WebSocketClient webSocketClient)
        {
            _title = "Authorization";
            _clientState = state;
            _dialogService = dialogService;
            _webSocketClient = webSocketClient;
            _webSocketClient.Connected += AuthorizeUserCommand.RaiseCanExecuteChanged;
            _webSocketClient.AuthorizationResponseСame += ShowAuthorizationResult;
            _webSocketClient.Connect();
        }

        private DelegateCommand<object> _authorizeUserCommand;
        public DelegateCommand<object> AuthorizeUserCommand => _authorizeUserCommand ?? (_authorizeUserCommand = new DelegateCommand<object>(AuthorizeUserExecute, AuthorizeUserCanExecute));

        private void AuthorizeUserExecute(object obj)
        {
            _webSocketClient.Authorize(Login);
           // _clientState.AuthorizeUser(new User(Login, OnlineStatus.Online));
            CloseDialogCommand.Execute();
        }

        private bool AuthorizeUserCanExecute(object obj)
        {
            if (Login != null && Login != "" && _webSocketClient.IsConnected==true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }




        private DelegateCommand _closeDialogCommand;
        public DelegateCommand CloseDialogCommand
        {
            get
            {
                if (_closeDialogCommand != null)
                {
                    return _closeDialogCommand;
                }
                else
                {
                    return _closeDialogCommand = new DelegateCommand(CloseDialog);
                }
            }
        }

        public event Action<IDialogResult> RequestClose;

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }
        protected virtual void CloseDialog()
        {
            ButtonResult result = ButtonResult.None;
            RaiseRequestClose(new DialogResult(result));
        }
        public virtual bool CanCloseDialog()
        {
            return true;
        }
        public virtual void OnDialogClosed()
        {

        }

        private void ShowAuthorizationResult(AuthorizationResponse response)
        {
            _dialogService.ShowDialog("NotificationWindow");
        }

        private DelegateCommand _showServerConfigCommand;
        public DelegateCommand ShowServerConfigCommand => _showServerConfigCommand ?? (_showServerConfigCommand = new DelegateCommand(ShowServerConfigExecute));

        private void ShowServerConfigExecute()
        {
            _dialogService.ShowDialog("ServerConfigDialog");
        }
        public virtual void OnDialogOpened(IDialogParameters parameters)
        {

        }
    }
}
