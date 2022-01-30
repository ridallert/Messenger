using Messenger.Common;
using Messenger.Network.EventArgs;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;
using Messenger.Network.Requests;
using Messenger.Network.Responses;
using System.Windows;
using System.Net;
using Messenger.Models;
using Messenger.Network.Broadcasts;

namespace Messenger.Network
{
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
        public event Action<GetContactsResponse> GetContactsResponseСame;
        public event Action<GetChatListResponse> GetChatListResponseСame;

        public event Action<GetPrivateMessageListResponse> GetPrivateMessageListResponseCame;
        public event Action<GetPublicMessageListResponse> GetPublicMessageListResponseCame;
        public event Action<PrivateMessageReceivedResponse> PrivateMessageReceivedResponseCame;
        public event Action<PublicMessageReceivedResponse> PublicMessageReceivedResponseCame;
        public event Action<UserStatusChangedBroadcast> UserStatusChangedBroadcastCame;
        public event Action<GetEventListResponse> GetEventListResponseCame;

        public event Action Connected;
        public event Action Disconnected;

        //public event EventHandler MessageReceived;
        //public event EventHandler<ConnectionStateChangedEventArgs> Connected;
        //public event EventHandler<ConnectionStateChangedEventArgs> Disconnected;



        public WebSocketClient()
        {
            _socket = new WebSocket($"ws://127.0.0.1:7890");
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

                case nameof(GetContactsResponse):
                    GetContactsResponse getUserListResponse = JsonConvert.DeserializeObject<GetContactsResponse>(container.Payload.ToString());
                    GetContactsResponseСame?.Invoke(getUserListResponse);
                    break;
                case nameof(GetChatListResponse):
                    GetChatListResponse getChatListResponse = JsonConvert.DeserializeObject<GetChatListResponse>(container.Payload.ToString());
                    GetChatListResponseСame?.Invoke(getChatListResponse);
                    break;

                case nameof(GetPrivateMessageListResponse):
                    GetPrivateMessageListResponse getPrivateMessageListResponse = JsonConvert.DeserializeObject<GetPrivateMessageListResponse>(container.Payload.ToString());
                    GetPrivateMessageListResponseCame?.Invoke(getPrivateMessageListResponse);
                    break;

                case nameof(PrivateMessageReceivedResponse):
                    PrivateMessageReceivedResponse privateMessageDeliveredResponse = JsonConvert.DeserializeObject<PrivateMessageReceivedResponse>(container.Payload.ToString());
                    PrivateMessageReceivedResponseCame?.Invoke(privateMessageDeliveredResponse);
                    break;

                case nameof(PublicMessageReceivedResponse):
                    PublicMessageReceivedResponse publicMessageDeliveredResponse = JsonConvert.DeserializeObject<PublicMessageReceivedResponse>(container.Payload.ToString());
                    PublicMessageReceivedResponseCame?.Invoke(publicMessageDeliveredResponse);
                    break;

                case nameof(GetPublicMessageListResponse):
                    GetPublicMessageListResponse getPublicMessageListResponse = JsonConvert.DeserializeObject<GetPublicMessageListResponse>(container.Payload.ToString());
                    GetPublicMessageListResponseCame?.Invoke(getPublicMessageListResponse);
                    break;

                case nameof(UserStatusChangedBroadcast):
                    UserStatusChangedBroadcast userStatusChangedBroadcast = JsonConvert.DeserializeObject<UserStatusChangedBroadcast>(container.Payload.ToString());
                    UserStatusChangedBroadcastCame?.Invoke(userStatusChangedBroadcast);
                    break;
                case nameof(GetEventListResponse):
                    GetEventListResponse getEventListResponse = JsonConvert.DeserializeObject<GetEventListResponse>(container.Payload.ToString());
                    GetEventListResponseCame?.Invoke(getEventListResponse);
                    break;
            }
        }
        public void SetParams(string ipAddress, int port)
        {
            _socket = new WebSocket($"ws://{ipAddress}:{port}");
        }
        public void Connect()
        {
            _socket.OnOpen += OnOpen;
            _socket.OnMessage += OnMessage;
            _socket?.ConnectAsync();
        }
        public void Disconnect()
        {
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
        public void SendPrivateMessage(string sender, string receiver, string message, DateTime sendTime)
        {
            _sendQueue.Enqueue(new SendPrivateMessageRequest(sender, receiver, message, sendTime).GetContainer());

            if (Interlocked.CompareExchange(ref _sending, 1, 0) == 0)
                Send();
        }
        //public void SendPublicMessage(string sender, string message, DateTime sendTime)
        //{
        //    _sendQueue.Enqueue(new SendPublicMessageRequest(sender, message, sendTime).GetContainer());

        //    if (Interlocked.CompareExchange(ref _sending, 1, 0) == 0)
        //        Send();
        //}
        public void GetContacts(string name)
        {
            _sendQueue.Enqueue(new GetContactsRequest(name).GetContainer());

            if (Interlocked.CompareExchange(ref _sending, 1, 0) == 0)
                Send();
        }
        public void GetChatList(string name)
        {
            _sendQueue.Enqueue(new GetChatListRequest(name).GetContainer());

            if (Interlocked.CompareExchange(ref _sending, 1, 0) == 0)
                Send();
        }
        public void GetPrivateMessageList(string name)
        { 
            _sendQueue.Enqueue(new GetPrivateMessageListRequest(name).GetContainer());

            if (Interlocked.CompareExchange(ref _sending, 1, 0) == 0)
                Send();
        }
        public void GetPublicMessageList()
        {
            _sendQueue.Enqueue(new GetPublicMessageListRequest().GetContainer());

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
            Disconnected?.Invoke();
            //Disconnected?.Invoke(this, new ConnectionStateChangedEventArgs(_login, false));
        }
    }
}
