﻿using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.ViewModels
{
    class AuthorizationDialogViewModel : BindableBase, IDialogAware
    {
        //public string Title => throw new NotImplementedException();

        //public event Action<IDialogResult> RequestClose;

        //public bool CanCloseDialog()
        //{
        //    throw new NotImplementedException();
        //}

        //public void OnDialogClosed()
        //{
        //    throw new NotImplementedException();
        //}

        //public void OnDialogOpened(IDialogParameters parameters)
        //{
        //    throw new NotImplementedException();
        //}

        private string _title = "Notification";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public event Action<IDialogResult> RequestClose;

        public virtual bool CanCloseDialog()
        {
            return true;
        }

        public virtual void OnDialogClosed()
        {

        }

        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
            Message = parameters.GetValue<string>("message");
        }

        

        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }



        private DelegateCommand<string> _closeDialogCommand;
        public DelegateCommand<string> CloseDialogCommand
        {
            get
            {
                if (_closeDialogCommand != null)
                {
                    return _closeDialogCommand;
                }
                else
                {
                    return _closeDialogCommand = new DelegateCommand<string>(CloseDialog);
                }
            }
        }
     
            

        protected virtual void CloseDialog(string parameter)
        {
            ButtonResult result = ButtonResult.None;

            if (parameter?.ToLower() == "true")
                result = ButtonResult.OK;
            else if (parameter?.ToLower() == "false")
                result = ButtonResult.Cancel;

            RaiseRequestClose(new DialogResult(result));
        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }

       

        

        
    }
}