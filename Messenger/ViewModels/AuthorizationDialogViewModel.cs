﻿using Prism.Commands;
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
using System.Windows.Threading;
using System.Threading;
using System.Windows;

namespace Messenger.ViewModels
{
    class AuthorizationDialogViewModel : BindableBase, IDialogAware
    {
        private ClientStateManager _clientState;
        private WebSocketClient _webSocketClient;
        private IDialogService _dialogService;


        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private string _serverConfigButtonName;
        public string ServerConfigButtonName
        {
            get { return _serverConfigButtonName; }
            set { SetProperty(ref _serverConfigButtonName, value); }
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

        public AuthorizationDialogViewModel(IDialogService dialogService, ClientStateManager state, WebSocketClient webSocketClient)
        {
            _title = "Authorization";
            ServerConfigButtonName = "Server config (Disconnected)";
            _clientState = state;
            _dialogService = dialogService;
            _webSocketClient = webSocketClient;

            
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

        private DelegateCommand _authorizeUserCommand;
        public DelegateCommand AuthorizeUserCommand => _authorizeUserCommand ?? (_authorizeUserCommand = new DelegateCommand(AuthorizeUserExecute, AuthorizeUserCanExecute));

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

        private DelegateCommand _closeDialogCommand;
        public DelegateCommand CloseDialogCommand => _closeDialogCommand ?? (_closeDialogCommand = new DelegateCommand(CloseDialog));

        public event Action<IDialogResult> RequestClose;

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
            if (response.Result == "Already exists" || response.Result == "New user added")
            {
                Application.Current.Dispatcher.InvokeAsync(CloseDialog);
                //_clientState.AuthorizeUser(response.Name, response.UserId);
                _webSocketClient.GetContacts(response.UserId);
                _webSocketClient.GetChatList(response.UserId);
                _webSocketClient.GetEventLog(DateTime.Today.AddDays(-1), DateTime.Today);
            }
            Application.Current.Dispatcher.InvokeAsync(() => ShowNotificationWindow(response));
        }

        // Action callBack;
        private void ShowNotificationWindow(AuthorizationResponse response)
        {
            var par = new DialogParameters
            {
                { "name", response.Name },
                { "result", response.Result }
            };

            _dialogService.ShowDialog("NotificationWindow", par, Callback);
        }
        void Callback(IDialogResult result)
        {
            //tcs.SetResult(result.Parameters.GetValue<bool>("confirmed"));
        }

        private DelegateCommand _showServerConfigCommand;
        public DelegateCommand ShowServerConfigCommand => _showServerConfigCommand ?? (_showServerConfigCommand = new DelegateCommand(ShowServerConfigExecute));

        private void ShowServerConfigExecute()
        {
            _dialogService.ShowDialog("ServerConfigDialog");
        }

    }
}
