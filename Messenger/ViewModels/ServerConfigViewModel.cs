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
        private ClientStateManager _clientState;
        private WebSocketClient _webSocketClient; //-----------

        private ObservableCollection<Protocol> _interfaceItems;
        private Protocol _interfaceSelectedItem;
        private string _address;
        private int _port;

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
        public string Address
        {
            get { return _address; }
            set { SetProperty(ref _address, value); }
        }
        public int Port
        {
            get { return _port; }
            set { SetProperty(ref _port, value); }
        }


        public ServerConfigViewModel(ClientStateManager state, WebSocketClient webSocketClient)
        {
            _clientState = state;
            _webSocketClient = webSocketClient; //------
            InterfaceItems = new ObservableCollection<Protocol>();
            InterfaceItems.Add(Protocol.WS);
            //InterfaceItems.Add(Protocol.TCP);

            InterfaceSelectedItem = Protocol.WS;
            Address = IPAddress.Loopback.ToString();
            Port = 7890;
        }


        private DelegateCommand _connectCommand;
        public DelegateCommand ConnectCommand => _connectCommand ?? (_connectCommand = new DelegateCommand(ConnectExecute, ConnectCanExecute));

        private void ConnectExecute()
        {
            _webSocketClient.SetParams(Address, Port);
            _webSocketClient.Connect();
            CloseDialogCommand.Execute();
        }
        private bool ConnectCanExecute()
        {
            return _webSocketClient != null;
        }








        //CLOSING WINDOW

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








        private string _title = "ServerConfig";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        public void OnDialogClosed()
        {
            
        }
        public void OnDialogOpened(IDialogParameters parameters)
        {

        }
       
    }
}
