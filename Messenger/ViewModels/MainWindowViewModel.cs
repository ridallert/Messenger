using Messenger.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Messenger.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private IDialogService _dialogService;

        private string _title;

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public string _loginButtonContent;
        public string LoginButtonContent
        {
            get { return _loginButtonContent; }
            set
            {
                SetProperty(ref _loginButtonContent, value);
            }
        }
        public MainWindowViewModel(IDialogService dialogService)
        {
            Title = "PrismMessenger";

            LoginButtonContent = "Log in";

            
            _dialogService = dialogService;

            State.UserAuthorized += OnUserAuthorized;
            State.UserLoggedOut += OnUserLoggedOut;

        }

        private void OnUserAuthorized()
        {
            LoginButtonContent = "Log out";
        }

        private void OnUserLoggedOut()
        {
            LoginButtonContent = "Log in";
        }


        private DelegateCommand _showAuthDialCommand;       
        public DelegateCommand ShowAuthorizationDialogCommand => _showAuthDialCommand ?? (_showAuthDialCommand = new DelegateCommand(ShowAuthDialogExecute));  

        private void ShowAuthDialogExecute()              
        {
            if (State.AuthorizedUser == null)
            {
                _dialogService.ShowDialog("AuthorizationDialog");
            }
            else
            {
                State.AuthorizedUser = null;
            }

            

            //string message = "This is a message that should be shown in the dialog.";
            //_dialogService.ShowDialog("AuthorizationDialog", new DialogParameters($"message={message}"),
            //r =>
            //{
            //    if (r.Result == ButtonResult.None)
            //        Title = "Result is None";
            //    else if (r.Result == ButtonResult.OK)
            //        Title = "Result is OK";
            //    else if (r.Result == ButtonResult.Cancel)
            //        Title = "Result is Cancel";
            //    else
            //        Title = "I Don't know what you did!?";
            //});
        }
    }
}
