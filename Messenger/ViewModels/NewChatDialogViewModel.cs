using Messenger.Common;
using Messenger.Models;
using Messenger.Network;
using Prism.Commands;
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
    class NewChatDialogViewModel : BindableBase, IDialogAware
    {
        private ClientStateManager _clientState;
        private WebSocketClient _webSocketClient;

        private string _chatTitle;
        private ObservableCollection<User> _users;
        public string ChatTitle
        {
            get { return _chatTitle; }
            set { SetProperty(ref _chatTitle, value); }
        }
        public ObservableCollection<User> Users
        {
            get { return _users; }
            set { SetProperty(ref _users, value); }
        }
        

        public NewChatDialogViewModel(ClientStateManager clientState, WebSocketClient webSocketClient)
        {
            _webSocketClient = webSocketClient;
            _clientState = clientState;
            Title = "Event log";
            Users = _clientState.GetContactList();
        }

        

        private DelegateCommand<IList<object>> _createCommand;
        public DelegateCommand<IList<object>> CreateCommand => _createCommand ?? (_createCommand = new DelegateCommand<IList<object>>(CreateExecute, CreateCanExecute));

        private void CreateExecute(IList<object> selectedItems)
        {
            List<int> idList = new List<int>();
            foreach (object item in selectedItems)
            {
                idList.Add(((User)item).UserId);
                idList.Add(_clientState.UserId.Value);
            }

            _webSocketClient.SendCreateNewChatRequest(ChatTitle, idList);
        }
        private bool CreateCanExecute(IList<object> selectedItems)
        {
            return selectedItems.Count != 0;
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
           
        }
    }
}
