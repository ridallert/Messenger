namespace Messenger.Network.Broadcasts
{
    using Messenger.DataObjects;

    public class UserStatusChangedBroadcast
    {
        #region Properties

        public string Name { get; set; }

        public int UserId { get; set; }

        public UserStatus Status { get; set; }

        #endregion //Properties

        #region Constructors

        public UserStatusChangedBroadcast(string name, int userId, UserStatus status)
        {
            Name = name;
            UserId = userId;
            Status = status;
        }

        #endregion //Constructors

        #region Methods

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(UserStatusChangedBroadcast),
                Payload = this
            };

            return container;
        }

        #endregion //Methods
    }
}
