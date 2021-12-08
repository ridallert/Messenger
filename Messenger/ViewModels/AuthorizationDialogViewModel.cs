using Prism.Commands;
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

        private string _title = "Authorization";
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

        protected virtual void CloseDialog()
        {
            ButtonResult result = ButtonResult.None;
            RaiseRequestClose(new DialogResult(result));
        }
        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }

        public virtual void OnDialogClosed()
        {

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

        protected virtual void CloseDialog(string parameter)
        {
            ButtonResult result = ButtonResult.None;

            //if (parameter?.ToLower() == "true")
            //    result = ButtonResult.OK;
            //else if (parameter?.ToLower() == "false")
            //    result = ButtonResult.Cancel;

            RaiseRequestClose(new DialogResult(result));
        }

        // Открыть ServerConfigDialog
        public AuthorizationDialogViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        private IDialogService _dialogService;

        private DelegateCommand<object> _showServerConfigCommand;        //Удалить <object>?
        public DelegateCommand<object> ShowServerConfigCommand => _showServerConfigCommand ?? (_showServerConfigCommand = new DelegateCommand<object>(ShowServerConfigExecute));        //Удалить <object>?


        private void ShowServerConfigExecute(object obj)        //Удалить <object>?
        {
            _dialogService.ShowDialog("ServerConfigDialog");
        }
    }
}
