using Messenger.Common;
using Messenger.Models;
using Messenger.Network;
using Messenger.Network.Responses;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Messenger.ViewModels
{
    class NewChatDialogViewModel : BindableBase, IDialogAware, IDataErrorInfo
    {
        private ClientStateManager _clientState;
        private WebSocketClient _webSocketClient;
        private IDialogService _dialogService;

        private string _chatTitle;
        private Visibility _isTitleVisible;
        private string _notificationText;
        private Visibility _isNotificationVisible;
        private ObservableCollection<User> _availableUsers;
        private ObservableCollection<User> _selectedUsers;
        private User _availableUsersSelected;
        private User _selectedUsersSelected;

        public string ChatTitle
        {
            get { return _chatTitle; }
            set
            {
                SetProperty(ref _chatTitle, value);
                CreateCommand.RaiseCanExecuteChanged();
            }
        }
        public Visibility IsTitleVisible
        {
            get { return _isTitleVisible; }
            set { SetProperty(ref _isTitleVisible, value); }
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

        public NewChatDialogViewModel(ClientStateManager clientState, WebSocketClient webSocketClient, IDialogService dialogService)
        {
            _dialogService = dialogService;
            _webSocketClient = webSocketClient;
            _clientState = clientState;
            Title = "Event log";
            IsTitleVisible = Visibility.Hidden;
            IsNotificationVisible = Visibility.Hidden;
            AvailableUsers = _clientState.GetContactList();
            SelectedUsers = new ObservableCollection<User>();
        }

        private void OnUserStatusChanged(User newUser)
        {
            bool isUserExist=false;
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
                AvailableUsers.Add(newUser);
            }
        }

        private DelegateCommand _selectUserCommand;
        public DelegateCommand SelectUserCommand => _selectUserCommand ?? (_selectUserCommand = new DelegateCommand(SelectUserExecute, SelectUserCanExecute));

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

        private DelegateCommand _removeUserCommand;
        public DelegateCommand RemoveUserCommand => _removeUserCommand ?? (_removeUserCommand = new DelegateCommand(RemoveUserExecute, RemoveUserCanExecute));

        private void RemoveUserExecute()
        {
            AvailableUsers.Add(SelectedUsersSelectedItem);
            SelectedUsers.Remove(SelectedUsersSelectedItem);
            CreateCommand.RaiseCanExecuteChanged();
            if (SelectedUsers == null || SelectedUsers.Count <= 1)
            {
                ChatTitle = null;
                IsTitleVisible = Visibility.Hidden;
            }
        }
        private bool RemoveUserCanExecute()
        {
            return SelectedUsersSelectedItem != null;
        }


        private DelegateCommand _createCommand;
        public DelegateCommand CreateCommand => _createCommand ?? (_createCommand = new DelegateCommand(CreateExecute, CreateCanExecute));

        private void CreateExecute()
        {
            List<int> idList = new List<int>();
            foreach (User user in SelectedUsers)
            {
                idList.Add(user.UserId);
            }
            idList.Add(_clientState.UserId.Value);

            _webSocketClient.SendCreateNewChatRequest(ChatTitle, idList);

        }
        private bool CreateCanExecute()
        {
            if (SelectedUsers != null && SelectedUsers.Count != 0)
            {
                if (SelectedUsers.Count > 1)
                {
                    IsTitleVisible = Visibility.Visible;
                    if (ChatTitle == null || ChatTitle == "")
                        return false;
                }
                else
                {
                    IsTitleVisible = Visibility.Hidden;
                }
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

        private DelegateCommand _closeDialogCommand;
        public DelegateCommand CloseDialogCommand => _closeDialogCommand ?? (_closeDialogCommand = new DelegateCommand(CloseDialog));
        

        //IDataErrorInfo
        public string Error => throw new Exception("Chat title validation error");

        public string this[string propertyName]
        {
            get
            {
                string validationResult = String.Empty;
                if (propertyName == "ChatTitle")
                {
                    if (String.IsNullOrEmpty(ChatTitle))
                    {
                        validationResult = "Chat title cannot be empty";
                    }
                    if (_clientState.IsChatNameTaken(ChatTitle))
                    {
                        validationResult = "Chat name is taken";
                    }
                }
                return validationResult;
            }
        }

        //IDialogAware
        private string _title;
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
            _webSocketClient.CreateNewChatResponseСame += ShowResult;
            _clientState.UserStatusChanged += OnUserStatusChanged;
        }
        protected virtual void CloseDialog()
        {
            ButtonResult result = ButtonResult.None;
            RaiseRequestClose(new DialogResult(result));
        }
        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
            _webSocketClient.CreateNewChatResponseСame -= ShowResult;
            _clientState.UserStatusChanged -= OnUserStatusChanged;
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

            _dialogService.ShowDialog("NotificationWindow", par, Callback);
            CloseDialog();
        }
        void Callback(IDialogResult result)
        {
            //tcs.SetResult(result.Parameters.GetValue<bool>("confirmed"));
        }
    }
}
