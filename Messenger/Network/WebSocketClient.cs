namespace Messenger.Network
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using WebSocketSharp;
    using Messenger.Network.Requests;
    using Messenger.Network.Responses;
    using Messenger.Network.Broadcasts;

    public class WebSocketClient
    {
        private readonly ConcurrentQueue<MessageContainer> _sendQueue;
        private int _sending;

        private WebSocket _socket;
        public bool IsConnected
        {
            get
            {
                return _socket?.ReadyState == WebSocketState.Open;
            }
        }

        public event Action<AuthorizationResponse> AuthorizationResponseСame;
        public event Action<GetUserListResponse> GetUserListResponseСame;
        public event Action<GetChatListResponse> GetChatListResponseСame;
        public event Action<CreateNewChatResponse> CreateNewChatResponseСame;
        public event Action<NewChatCreatedResponse> NewChatCreatedResponseСame;

        public event Action<MessageReceivedResponse> MessageReceivedResponseCame;
        public event Action<UserStatusChangedBroadcast> UserStatusChangedBroadcastCame;
        public event Action<GetEventListResponse> GetEventListResponseCame;

        public event Action Connected;
        public event Action Disconnected;

        //public event EventHandler MessageReceived;
        //public event EventHandler<ConnectionStateChangedEventArgs> Connected;
        //public event EventHandler<ConnectionStateChangedEventArgs> Disconnected;

        public WebSocketClient()
        {
            
            _sendQueue = new ConcurrentQueue<MessageContainer>();
            _sending = 0;
        }

        private void Send()
        {
            if (!IsConnected)
                return;
            if (!_sendQueue.TryDequeue(out var message) && Interlocked.CompareExchange(ref _sending, 0, 1) == 1)
                return;
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            string serializedMessages = JsonConvert.SerializeObject(message, settings);
            _socket.SendAsync(serializedMessages, SendCompleted);
        }
        private void SendCompleted(bool completed)
        {
            if (!completed)
            {
                Disconnect();
                Disconnected?.Invoke();
                //Disconnected?.Invoke(this, new ConnectionStateChangedEventArgs(_login, false));
                return;
            }
            Send();
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
            if (e.IsText == false)
                return;

            MessageContainer container = JsonConvert.DeserializeObject<MessageContainer>(e.Data);

            switch (container.Identifier)
            {
                case nameof(AuthorizationResponse):
                    AuthorizationResponse authorizationResponse = JsonConvert.DeserializeObject<AuthorizationResponse>(container.Payload.ToString());
                    AuthorizationResponseСame?.Invoke(authorizationResponse);
                    break;

                case nameof(UserStatusChangedBroadcast):
                    UserStatusChangedBroadcast userStatusChangedBroadcast = JsonConvert.DeserializeObject<UserStatusChangedBroadcast>(container.Payload.ToString());
                    UserStatusChangedBroadcastCame?.Invoke(userStatusChangedBroadcast);
                    break;

                case nameof(GetUserListResponse):
                    GetUserListResponse getUserListResponse = JsonConvert.DeserializeObject<GetUserListResponse>(container.Payload.ToString());
                    GetUserListResponseСame?.Invoke(getUserListResponse);
                    break;

                case nameof(GetChatListResponse):
                    GetChatListResponse getChatListResponse = JsonConvert.DeserializeObject<GetChatListResponse>(container.Payload.ToString());
                    GetChatListResponseСame?.Invoke(getChatListResponse);
                    break;

                case nameof(CreateNewChatResponse):
                    CreateNewChatResponse createNewChatResponse = JsonConvert.DeserializeObject<CreateNewChatResponse>(container.Payload.ToString());
                    CreateNewChatResponseСame?.Invoke(createNewChatResponse);
                    break;

                case nameof(NewChatCreatedResponse):
                    NewChatCreatedResponse newChatCreatedResponse = JsonConvert.DeserializeObject<NewChatCreatedResponse>(container.Payload.ToString());
                    NewChatCreatedResponseСame?.Invoke(newChatCreatedResponse);
                    break;

                case nameof(MessageReceivedResponse):
                    MessageReceivedResponse messageReceivedResponse = JsonConvert.DeserializeObject<MessageReceivedResponse>(container.Payload.ToString());
                    MessageReceivedResponseCame?.Invoke(messageReceivedResponse);
                    break;

                case nameof(GetEventListResponse):
                    GetEventListResponse getEventListResponse = JsonConvert.DeserializeObject<GetEventListResponse>(container.Payload.ToString());
                    GetEventListResponseCame?.Invoke(getEventListResponse);
                    break;
            }
        }
        //public void SetParams(string ipAddress, int port)
        //{
        //    _socket = new WebSocket($"ws://{ipAddress}:{port}");
        //}
        public void Connect(string address, int port)
        {
            if (IsConnected)
            {
                Disconnect();
            }
            _socket = new WebSocket($"ws://{address}:{port}");
            _socket.OnOpen += OnOpen;
            _socket.OnClose += OnClose;
            _socket.OnMessage += OnMessage;
            
            _socket?.ConnectAsync();
        }



        public void Disconnect()
        {
            _socket.OnOpen -= OnOpen;
            _socket.OnClose -= OnClose;
            _socket.OnMessage -= OnMessage;
            _socket?.CloseAsync();
        }

        public void Authorize(string login)
        {
            _sendQueue.Enqueue(new AuthorizationRequest(login).GetContainer());

            if (Interlocked.CompareExchange(ref _sending, 1, 0) == 0)
                Send();
        }
        public void SendCreateNewChatRequest(string title, List<int> userIdList)
        {
            _sendQueue.Enqueue(new CreateNewChatRequest(title, userIdList).GetContainer());

            if (Interlocked.CompareExchange(ref _sending, 1, 0) == 0)
                Send();
        }
        public void SendMessage(int senderId, int chatId, string message, DateTime sendTime)
        {
            _sendQueue.Enqueue(new SendMessageRequest(senderId, chatId, message, sendTime).GetContainer());

            if (Interlocked.CompareExchange(ref _sending, 1, 0) == 0)
                Send();
        }

        public void GetContacts(int userId)
        {
            _sendQueue.Enqueue(new GetUserListRequest(userId).GetContainer());

            if (Interlocked.CompareExchange(ref _sending, 1, 0) == 0)
                Send();
        }
        public void GetChatList(int userId)
        {
            _sendQueue.Enqueue(new GetChatListRequest(userId).GetContainer());

            if (Interlocked.CompareExchange(ref _sending, 1, 0) == 0)
                Send();
        }
       
        public void GetEventLog(DateTime from, DateTime to)
        {
            _sendQueue.Enqueue(new GetEventListRequest(from, to).GetContainer());

            if (Interlocked.CompareExchange(ref _sending, 1, 0) == 0)
                Send();
        }
        private void OnOpen(object sender, System.EventArgs e)
        {
            Connected?.Invoke();
            //Connected?.Invoke(this, new ConnectionStateChangedEventArgs(_login, true));
        }
        private void OnClose(object sender, CloseEventArgs e)
        {
            Disconnect();
            Disconnected?.Invoke();
            //Disconnected?.Invoke(this, new ConnectionStateChangedEventArgs(_login, false));
        }
    }
}
