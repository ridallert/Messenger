namespace Messenger.DataObjects
{
    public class ChatPresenter : Chat
    {
        #region Fields

        private UserStatus? _isOnline;
        private int? _newMessageCounter;

        #endregion //Fields

        #region Properties

        public UserStatus? IsOnline
        {
            get { return _isOnline; }
            set { SetProperty(ref _isOnline, value); }
        }
        
        public int? NewMessageCounter
        {
            get { return _newMessageCounter; }
            set { SetProperty(ref _newMessageCounter, value); }
        }

        #endregion //Properties
    }
}