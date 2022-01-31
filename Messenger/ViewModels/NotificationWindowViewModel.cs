using Messenger.Network;
using Messenger.Network.Responses;
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
    public class NotificationWindowViewModel : BindableBase, IDialogAware
    {
        private string _message;

        //private WebSocketClient _webSocketClient;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        private string _title = "NotificationWindow";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        //public NotificationWindowViewModel() { }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            if (parameters.ContainsKey("result"))
            {
                string name = parameters.GetValue<string>("name");
                string response = parameters.GetValue<string>("result");

                Message = response;//

                if (response == "NameIsTaken")
                    Message = "Name '" + name + "' is taken";
                if (response == "AlreadyExists")
                    Message = "You logged in as '" + name + "'";
                if (response == "NewUserAdded")
                    Message = "User '" + name + "' is added";
            }
        }
        public void OnDialogClosed()
        {

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
    }
}
