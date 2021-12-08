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

        public MainWindowViewModel(IDialogService dialogService)
        {  
            _dialogService = dialogService;
            Title = "Prism Application";
        }

        private DelegateCommand<object> _showAuthorizationDialogCommand;        //Удалить <object>?
        public DelegateCommand<object> ShowAuthorizationDialogCommand => _showAuthorizationDialogCommand ?? (_showAuthorizationDialogCommand = new DelegateCommand<object>(ShowAuthorizationDialogExecute));        //Удалить <object>?

        private void ShowAuthorizationDialogExecute(object obj)        //Удалить <object>?
        {
            //string message = "This is a message that should be shown in the dialog.";

            _dialogService.ShowDialog("AuthorizationDialog");

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
