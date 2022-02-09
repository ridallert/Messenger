namespace Messenger.ViewModels
{
    using Prism.Mvvm;
    using Prism.Services.Dialogs;
    using System;
    using System.Timers;
    using System.Windows;

    public class NotificationWindowViewModel : BindableBase, IDialogAware
    {
        private Timer _timer; 
        private string _message;
        
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        private string _title = "NotificationWindow";

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            _timer = new Timer { AutoReset = false, Interval = 1000 };
            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();

            if (parameters.ContainsKey("result"))
            {
                Message = parameters.GetValue<string>("result");
            }
        }
        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.InvokeAsync(CloseDialog);
        }

        public void OnDialogClosed()
        {

        }

        public event Action<IDialogResult> RequestClose;

        public virtual bool CanCloseDialog()
        {
            return true;
        }

        protected virtual void CloseDialog()
        {
            ButtonResult result = ButtonResult.None;
            RaiseRequestClose(new DialogResult(result));
        }
        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }
    }
}
