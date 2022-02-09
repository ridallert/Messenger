namespace Messenger.Common
{
    using Newtonsoft.Json;
    using Prism.Mvvm;

    public class User: BindableBase
    {
        private OnlineStatus _isOnline;
        public int UserId { get; set; }
        public string Name { get; set; }
        public OnlineStatus IsOnline
        {
            get { return _isOnline; }
            set { SetProperty(ref _isOnline, value); }
        }

        [JsonConstructor]
        public User(int userId, string name, OnlineStatus isOnline) : this(name, isOnline)
        {
            UserId = userId;
        }
        public User(string name, OnlineStatus isOnline) : this()
        {
            Name = name;
            IsOnline = isOnline;
        }
        public User() {}
    }
}
