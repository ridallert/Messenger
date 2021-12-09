﻿using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messenger.Models;

namespace Messenger.ViewModels
{
    class AuthorizationDialogViewModel : BindableBase, IDialogAware
    {
        private string _title = "Authorization";
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

        private DelegateCommand<object> _authorizeUserCommand;
        public DelegateCommand<object> AuthorizeUserCommand => _authorizeUserCommand ?? (_authorizeUserCommand = new DelegateCommand<object>(AuthorizeUserExecute, AuthorizeUserCanExecute));

        private void AuthorizeUserExecute(object obj)
        {
            User newUser = new User(Login, OnlineStatus.Online);
            State.AuthorizedUser = newUser;
            State.Users.Add(newUser);
            CloseDialogCommand.Execute();
        }

        private bool AuthorizeUserCanExecute(object obj)
        {
            if (Login != null && Login != "")
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






        public AuthorizationDialogViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        private IDialogService _dialogService;

        private DelegateCommand _showServerConfigCommand;        
        public DelegateCommand ShowServerConfigCommand => _showServerConfigCommand ?? (_showServerConfigCommand = new DelegateCommand(ShowServerConfigExecute));       


        private void ShowServerConfigExecute()        
        {
            _dialogService.ShowDialog("ServerConfigDialog");
        }
        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
            //Message = parameters.GetValue<string>("message");
        }

        //private string _message;
        //public string Message
        //{
        //    get { return _message; }
        //    set { SetProperty(ref _message, value); }
        //}
        //protected virtual void CloseDialog(string parameter)
        //{
        //    ButtonResult result = ButtonResult.None;

        //    if (parameter?.ToLower() == "true")
        //        result = ButtonResult.OK;
        //    else if (parameter?.ToLower() == "false")
        //        result = ButtonResult.Cancel;

        //    RaiseRequestClose(new DialogResult(result));
        //}
    }
}
