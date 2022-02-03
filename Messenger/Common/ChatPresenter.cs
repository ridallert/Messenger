namespace Messenger.Common
{
    public class ChatPresenter : Chat
    {
        private OnlineStatus? _isOnline;
        public OnlineStatus? IsOnline
        {
            get { return _isOnline; }
            set { SetProperty(ref _isOnline, value); }
        }
        private int? _newMessageCounter;
        public int? NewMessageCounter
        {
            get { return _newMessageCounter; }
            set { SetProperty(ref _newMessageCounter, value); }
        }
    }
}