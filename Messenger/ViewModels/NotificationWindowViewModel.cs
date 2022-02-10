namespace Messenger.ViewModels
{
    using System;
    using System.Timers;
    using System.Windows;

    using Prism.Mvvm;
    using Prism.Services.Dialogs;

    public class NotificationWindowViewModel : BindableBase, IDialogAware
    {
        #region Fields

        private string _title = "NotificationWindow";
        private Timer _timer; 
        private string _message;

        #endregion //Fields

        #region Properties

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        #endregion //Properties

        #region Events

        public event Action<IDialogResult> RequestClose;

        #endregion //Events

        #region Methods

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

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.InvokeAsync(CloseDialog);
        }

        #endregion //Methods
    }
}
