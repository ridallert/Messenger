namespace Messenger.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows;

    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Services.Dialogs;

    using Messenger.DataObjects;
    using Messenger.Models;
    using Messenger.Network;
    using Messenger.Network.Responses;

    class NewChatDialogViewModel : BindableBase, IDialogAware
    {
        #region Fields

        private readonly ClientStateManager _clientState;
        private readonly WebSocketClient _webSocketClient;
        private readonly IDialogService _dialogService;
        private string _title;
        private string _notificationText;
        private Visibility _isNotificationVisible;
        private ObservableCollection<User> _availableUsers;
        private ObservableCollection<User> _selectedUsers;
        private User _availableUsersSelected;
        private User _selectedUsersSelected;
        private DelegateCommand _selectUserCommand;
        private DelegateCommand _removeUserCommand;
        private DelegateCommand _createCommand;
        private DelegateCommand _closeDialogCommand;

        #endregion //Fields

        #region Properties

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public string NotificationText
        {
            get { return _notificationText; }
            set { SetProperty(ref _notificationText, value); }
        }

        public Visibility IsNotificationVisible
        {
            get { return _isNotificationVisible; }
            set { SetProperty(ref _isNotificationVisible, value); }
        }

        public ObservableCollection<User> AvailableUsers
        {
            get { return _availableUsers; }
            set { SetProperty(ref _availableUsers, value); }
        }

        public ObservableCollection<User> SelectedUsers
        {
            get { return _selectedUsers; }
            set { SetProperty(ref _selectedUsers, value); }
        }

        public User AvailableUsersSelectedItem
        {
            get { return _availableUsersSelected; }
            set
            {
                SetProperty(ref _availableUsersSelected, value);
                SelectUserCommand.RaiseCanExecuteChanged();
            }
        }

        public User SelectedUsersSelectedItem
        {
            get { return _selectedUsersSelected; }
            set
            {
                SetProperty(ref _selectedUsersSelected, value);
                RemoveUserCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand SelectUserCommand => _selectUserCommand ??
            (_selectUserCommand = new DelegateCommand(SelectUserExecute, SelectUserCanExecute));

        public DelegateCommand RemoveUserCommand => _removeUserCommand ??
            (_removeUserCommand = new DelegateCommand(RemoveUserExecute, RemoveUserCanExecute));

        public DelegateCommand CreateCommand => _createCommand ??
            (_createCommand = new DelegateCommand(CreateExecute, CreateCanExecute));

        public DelegateCommand CloseDialogCommand => _closeDialogCommand ??
            (_closeDialogCommand = new DelegateCommand(CloseDialog));

        #endregion //Properties

        #region Events

        public event Action<IDialogResult> RequestClose;

        #endregion //Events

        #region Constructors

        public NewChatDialogViewModel(ClientStateManager clientState, WebSocketClient webSocketClient, IDialogService dialogService)
        {
            _dialogService = dialogService;
            _webSocketClient = webSocketClient;
            _clientState = clientState;
            Title = "Event log";
            IsNotificationVisible = Visibility.Hidden;
            AvailableUsers = _clientState.GetContactList();
            SelectedUsers = new ObservableCollection<User>();
        }

        #endregion //Constructors

        #region Methods

        public void OnDialogOpened(IDialogParameters parameters)
        {
            _webSocketClient.CreateNewChatResponseСame += ShowResult;
            _clientState.UserStatusChanged += OnUserStatusChanged;
            _clientState.NewChatAdded += OnNewChatAdded;
        }

        public void OnDialogClosed()
        {
            //Необходим для реализации IDialogAware
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
            _webSocketClient.CreateNewChatResponseСame -= ShowResult;
            _clientState.UserStatusChanged -= OnUserStatusChanged;
            _clientState.NewChatAdded -= OnNewChatAdded;
        }

        protected virtual void CloseDialog()
        {
            ButtonResult result = ButtonResult.None;
            RaiseRequestClose(new DialogResult(result));
        }

        private void OnNewChatAdded(ChatPresenter obj)
        {
            CreateCommand.RaiseCanExecuteChanged();
        }

        private void OnUserStatusChanged(User newUser)
        {
            if (newUser.IsOnline == UserStatus.Online)
            {
                bool isUserExist = false;

                foreach (User user in AvailableUsers)
                {
                    if (user.UserId == newUser.UserId)
                    {
                        user.IsOnline = newUser.IsOnline;
                        isUserExist = true;
                    }
                }
                if (!isUserExist)
                {
                    Application.Current.Dispatcher.InvokeAsync(() => AvailableUsers.Add(newUser));
                }
            }
        }

        private void SelectUserExecute()
        {
            SelectedUsers.Add(AvailableUsersSelectedItem);
            AvailableUsers.Remove(AvailableUsersSelectedItem);
            CreateCommand.RaiseCanExecuteChanged();
        }

        private bool SelectUserCanExecute()
        {
            return AvailableUsersSelectedItem != null;
        }

        private void RemoveUserExecute()
        {
            AvailableUsers.Add(SelectedUsersSelectedItem);
            SelectedUsers.Remove(SelectedUsersSelectedItem);
            CreateCommand.RaiseCanExecuteChanged();
        }

        private bool RemoveUserCanExecute()
        {
            return SelectedUsersSelectedItem != null;
        }

        private void CreateExecute()
        {
            string newChatTitle = null;

            if (SelectedUsers.Count > 1)
            {
                newChatTitle = _clientState.Login;

                foreach (User user in SelectedUsers)
                {
                    newChatTitle = $"{newChatTitle}, {user.Name}";
                }
            }

            List<int> idList = new List<int>();
            idList.Add(_clientState.UserId.Value);

            foreach (User user in SelectedUsers)
            {
                idList.Add(user.UserId);
            }

            _webSocketClient.SendCreateNewChatRequest(newChatTitle, idList);
        }

        private bool CreateCanExecute()
        {
            if (SelectedUsers != null && SelectedUsers.Count != 0)
            {
                if (_clientState.IsChatAlreadyExists(SelectedUsers))
                {
                    NotificationText = "Chat already exists";
                    IsNotificationVisible = Visibility.Visible;
                    return false;
                }
                else
                {
                    IsNotificationVisible = Visibility.Hidden;
                    return true;
                }
            }
            else
            {
                IsNotificationVisible = Visibility.Hidden;
                return false;
            }
        }

        private void ShowResult(CreateNewChatResponse response)
        {
            Application.Current.Dispatcher.InvokeAsync(CloseDialog);
            Application.Current.Dispatcher.InvokeAsync(() => ShowNotificationWindow(response));
        }

        private void ShowNotificationWindow(CreateNewChatResponse response)
        {
            var par = new DialogParameters
            {
                { "result", response.Result }
            };

            _dialogService.Show("NotificationWindow", par, Callback);
            CloseDialog();
        }

        private void Callback(IDialogResult result)
        {
            //Необходим для вызова диалогового окна
        }

        #endregion //Methods
    }
}
