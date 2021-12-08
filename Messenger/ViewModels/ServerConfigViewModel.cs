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
    class ServerConfigViewModel : BindableBase, IDialogAware
    {
        private string _title = "ServerConfig";
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
        public DelegateCommand CloseDialogCommand => _closeDialogCommand ?? (_closeDialogCommand = new DelegateCommand(CloseDialog));

        protected virtual void CloseDialog()
        {
            ButtonResult result = ButtonResult.None;
            RaiseRequestClose(new DialogResult(result));
        }
        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }

        public void OnDialogClosed()
        {
            
        }

        public void OnDialogOpened(IDialogParameters parameters)
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
