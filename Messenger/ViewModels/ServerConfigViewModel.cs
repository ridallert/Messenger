namespace Messenger.ViewModels
{
    using Messenger.Network;
    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Services.Dialogs;
    using System;
    using System.Net;

    class ServerConfigViewModel : BindableBase, IDialogAware
    {
        private WebSocketClient _webSocketClient;
        private string _address;
        private int _port;

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

        public ServerConfigViewModel(WebSocketClient webSocketClient)
        {
            _webSocketClient = webSocketClient;
            Address = IPAddress.Loopback.ToString();
            Port = 7890;
        }

        private DelegateCommand _connectCommand;
        public DelegateCommand ConnectCommand => _connectCommand ?? (_connectCommand = new DelegateCommand(ConnectExecute, ConnectCanExecute));

        private void ConnectExecute()
        {
            _webSocketClient.Connect(Address, Port);
            CloseDialogCommand.Execute();
        }

        private bool ConnectCanExecute()
        {
            return _webSocketClient != null;
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
