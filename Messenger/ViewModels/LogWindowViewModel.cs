namespace Messenger.ViewModels
{
    using System;
    using System.Collections.ObjectModel;

    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Services.Dialogs;

    using Messenger.DataObjects;
    using Messenger.Models;
    using Messenger.Network;

    class LogWindowViewModel : BindableBase, IDialogAware
    {
        #region Fields

        private readonly ClientStateManager _clientState;
        private readonly WebSocketClient _webSocketClient;
        private string _title = "Event log";
        private ObservableCollection<LogEntry> _eventList;
        private ObservableCollection<EventType> _eventTypes;
        private EventType _selectedEventType;
        private DateTime _from;
        private DateTime _to;
        private DelegateCommand _loadCommand;

        #endregion //Fields

        #region Properties

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

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

        public DelegateCommand LoadCommand => _loadCommand ??
            (_loadCommand = new DelegateCommand(LoadExecute, LoadCanExecute));

        #endregion //Properties

        #region Events

        public event Action<IDialogResult> RequestClose;

        #endregion //Events

        #region Constructors

        public LogWindowViewModel(ClientStateManager state, WebSocketClient webSocketClient)
        {
            _clientState = state;
            _webSocketClient = webSocketClient;
            _clientState.EventListChanged += OnEventListChanged;
            From = DateTime.Today.AddDays(-1);
            To = DateTime.Today;

            EventTypes = new ObservableCollection<EventType>();
            EventTypes.Add(EventType.All);
            EventTypes.Add(EventType.Event);
            EventTypes.Add(EventType.Message);
            EventTypes.Add(EventType.Error);

            SelectedEventType = EventType.All;
        }

        #endregion //Constructors

        #region Methods

        public void OnDialogOpened(IDialogParameters parameters)
        {
            EventList = new ObservableCollection<LogEntry>(_clientState.EventList);
        }

        public void OnDialogClosed()
        {
            //Необходим для реализации IDialogAware
        }
        
        public bool CanCloseDialog()
        {
            return true;
        }

        private void OnEventListChanged()
        {
            EventList = _clientState.GetEventLog(SelectedEventType);
        }

        private void LoadExecute()
        {
            _webSocketClient.GetEventLog(From, To.AddDays(1));
        }

        private bool LoadCanExecute()
        {
            return From <= To && To <= DateTime.Today;
        }

        #endregion //Methods
    }
}
