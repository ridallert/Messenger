using Messenger.Models;
using Messenger.Network;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.ViewModels
{
    class ServerConfigViewModel : BindableBase, IDialogAware
    {
        private State _serverState;

        private ObservableCollection<Protocol> _interfaceItems;
        private Protocol _interfaceSelectedItem;
        private IPAddress _address;
        private int? _port;

        public ObservableCollection<Protocol> InterfaceItems
        {
            get { return _interfaceItems; }
            set { SetProperty(ref _interfaceItems, value); }
        }
        public Protocol InterfaceSelectedItem
        {
            get { return _interfaceSelectedItem; }
            set { SetProperty(ref _interfaceSelectedItem, value); }
        }
        public IPAddress Address
        {
            get { return _address; }
            set { SetProperty(ref _address, value); }
        }
        public int? Port
        {
            get { return _port; }
            set { SetProperty(ref _port, value); }
        }



        public ServerConfigViewModel(State state)
        {
            _serverState = state;

            InterfaceItems = new ObservableCollection<Protocol>();
            InterfaceItems.Add(Protocol.WS);
            InterfaceItems.Add(Protocol.TCP);

            InterfaceSelectedItem = Protocol.WS;
            Address = IPAddress.Loopback;
            Port = 65000;
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

        private string _title = "ServerConfig";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
    }
}
