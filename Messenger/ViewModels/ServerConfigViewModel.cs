namespace Messenger.ViewModels
{
    using System;
    using System.Net;

    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Services.Dialogs;

    using Messenger.Network;

    class ServerConfigViewModel : BindableBase, IDialogAware
    {
        #region Fields

        private string _title = "ServerConfig";
        private WebSocketClient _webSocketClient;
        private string _address;
        private int _port;
        private DelegateCommand _connectCommand;
        private DelegateCommand _closeDialogCommand;

        #endregion //Fields

        #region Properties

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
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

        public DelegateCommand ConnectCommand => _connectCommand ??
            (_connectCommand = new DelegateCommand(ConnectExecute, ConnectCanExecute));

        public DelegateCommand CloseDialogCommand => _closeDialogCommand ??
            (_closeDialogCommand = new DelegateCommand(CloseDialog));

        #endregion //Properties

        #region Events

        public event Action<IDialogResult> RequestClose;

        #endregion //Events

        #region Constructors

        public ServerConfigViewModel(WebSocketClient webSocketClient)
        {
            _webSocketClient = webSocketClient;
            Address = IPAddress.Loopback.ToString();
            Port = 7890;
        }

        #endregion //Constructors

        #region Methods

        public void OnDialogOpened(IDialogParameters parameters)
        {
            //Необходим для реализации IDialogAware
        }

        public void OnDialogClosed()
        {
            //Необходим для реализации IDialogAware
        }

        public virtual bool CanCloseDialog()
        {
            return true;
        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }

        protected virtual void CloseDialog()
        {
            ButtonResult result = ButtonResult.None;
            RaiseRequestClose(new DialogResult(result));
        }

        private void ConnectExecute()
        {
            _webSocketClient.Connect(Address, Port);
            CloseDialogCommand.Execute();
        }

        private bool ConnectCanExecute()
        {
            return _webSocketClient != null;
        }

        #endregion //Methods
    }
}
