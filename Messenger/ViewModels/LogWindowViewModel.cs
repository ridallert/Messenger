namespace Messenger.ViewModels
{
    using Messenger.Common;
    using Messenger.Models;
    using Messenger.Network;
    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Services.Dialogs;
    using System;
    using System.Collections.ObjectModel;

    class LogWindowViewModel : BindableBase, IDialogAware
    {
        private ClientStateManager _clientState;
        private WebSocketClient _webSocketClient;
        private ObservableCollection<LogEntry> _eventList;

        private ObservableCollection<EventType> _eventTypes;
        private EventType _selectedEventType;
        private DateTime _from;
        private DateTime _to;

        public ObservableCollection<EventType> EventTypes
        {
            get { return _eventTypes; }
            set { SetProperty(ref _eventTypes, value); }
        }
        public EventType SelectedEventType
        {
            get { return _selectedEventType; }
            set { SetProperty(ref _selectedEventType, value); }
        }
        public DateTime From
        {
            get { return _from; }
            set
            {
                SetProperty(ref _from, value);
                LoadCommand.RaiseCanExecuteChanged();
            }
        }
        public DateTime To
        {
            get { return _to; }
            set
            {
                SetProperty(ref _to, value);
                LoadCommand.RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<LogEntry> EventList
        {
            get { return _eventList; }
            set { SetProperty(ref _eventList, value); }
        }
        public LogWindowViewModel(ClientStateManager state, WebSocketClient webSocketClient)
        {
            _clientState = state;
            _webSocketClient = webSocketClient;
            _clientState.EventListChanged += OnEventListChanged;
            From = DateTime.Today.AddDays(-1);
            To = DateTime.Today;
            _webSocketClient.GetEventLog(From, To);

            EventTypes = new ObservableCollection<EventType>();
            EventTypes.Add(EventType.All);
            EventTypes.Add(EventType.Event);
            EventTypes.Add(EventType.Message);
            EventTypes.Add(EventType.Error);

            SelectedEventType = EventType.All;
        }
        private void OnEventListChanged()
        {
            EventList = _clientState.GetEventLog(SelectedEventType);
        }

        private DelegateCommand _loadCommand;
        public DelegateCommand LoadCommand => _loadCommand ?? (_loadCommand = new DelegateCommand(LoadExecute, LoadCanExecute));

        private void LoadExecute()
        {
            _webSocketClient.GetEventLog(From, To.AddDays(1));
        }

        private bool LoadCanExecute()
        {
            return From <= To && To <= DateTime.Today;
        }

        //IDialogAware

        private string _title = "Event log";
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

        public void OnDialogClosed(){}

        public void OnDialogOpened(IDialogParameters parameters) {}
    }
}
