namespace Messenger.DataObjects
{
    using Newtonsoft.Json;

    using Prism.Mvvm;

    public class User: BindableBase
    {
        #region Fields

        private UserStatus _isOnline;

        #endregion //Fields

        #region Properties

        public int UserId { get; set; }

        public string Name { get; set; }

        public UserStatus IsOnline
        {
            get { return _isOnline; }
            set { SetProperty(ref _isOnline, value); }
        }

        #endregion //Properties

        #region Constructors

        [JsonConstructor]
        public User(int userId, string name, UserStatus isOnline)
        {
            UserId = userId;
            Name = name;
            IsOnline = isOnline;
        }

        #endregion //Constructors
    }
}
