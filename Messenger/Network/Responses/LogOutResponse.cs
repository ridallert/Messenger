namespace Messenger.Network.Responses
{
    class LogOutResponse
    {
        public string Result { get; set; }
        public string Name { get; set; } 

        public LogOutResponse(string result, string name)
        {
            Result = result;
            Name = name;
        }

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(LogOutResponse),
                Payload = this
            };

            return container;
        }
    }
}
