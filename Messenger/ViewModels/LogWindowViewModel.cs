using Messenger.Common;
using Messenger.Models;
using Messenger.Network;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.ViewModels
{
    class LogWindowViewModel : BindableBase, IDialogAware
    {
        private ClientStateManager _clientState;
        private WebSocketClient _webSocketClient;
        private ObservableCollection<LogEntry> _eventList;

        public ObservableCollection<LogEntry> EventList
        {
            get { return _eventList; }
            set { SetProperty(ref _eventList, value); }
        }
        public LogWindowViewModel(ClientStateManager state, WebSocketClient webSocketClient)
        {
            _clientState = state;
            _webSocketClient = webSocketClient;
            EventList = _clientState.GetEventLog();
            _clientState.EventListChanged += OnEventListChanged;
        }
        private void OnEventListChanged()
        {
            _clientState.GetEventLog();
        }

        //IDialogAware

        private string _title = "LogWindow";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {

        }
    }
}
