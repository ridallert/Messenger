namespace Messenger.Network.Responses
{
    public class SendMessageResponse
    {
        public string Result { get; set; }

        public SendMessageResponse(string result)
        {
            Result = result;
        }

        public MessageContainer GetContainer()
        {
            MessageContainer container = new MessageContainer
            {
                Identifier = nameof(SendMessageResponse),
                Payload = this
            };

            return container;
        }
    }
}
