﻿using Messenger.Common;
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

namespace Messenger.Network
{
    public class WebSocketClient
    {
        private ClientState _clientState;
        private readonly ConcurrentQueue<MessageContainer> _sendQueue;
        private int _sending;
        private string _login;

        private WebSocket _socket;
        public bool IsConnected
        {
            get
            {
                return _socket?.ReadyState == WebSocketState.Open;
            }
        }
        public event Action<AuthorizationResponse> AuthorizationResponseСame;
        public event Action Connected;
        public event Action Disconnected;

        //public event EventHandler<ConnectionStateChangedEventArgs> Connected;
        //public event EventHandler<ConnectionStateChangedEventArgs> Disconnected;

        public event EventHandler MessageReceived;

        public WebSocketClient(ClientState clientState)
        {
            _clientState = clientState;
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
                    if (authorizationResponse.Result == "NameIsTaken")
                    {
                        //
                    }
                    if (authorizationResponse.Result == "NewUserAdded")
                    {
                        _clientState.AddNewUser(authorizationResponse.Name);
                        _clientState.AuthorizeUser(authorizationResponse.Name);
                    }
                    if (authorizationResponse.Result == "AlreadyExists")
                    {
                        _clientState.AuthorizeUser(authorizationResponse.Name);
                    }   
                    break;
            }



            //Console.WriteLine(e.Data);
            //Console.WriteLine("Enter message:");
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
        public void SendPrivateMessage(User sender, User receiver, string message, DateTime sendTime)
        {
            _sendQueue.Enqueue(new SendPrivateMessageRequest(sender.Name, receiver.Name, message, sendTime).GetContainer());

            if (Interlocked.CompareExchange(ref _sending, 1, 0) == 0)
                Send();
        }
        public void GetUserList()
        {
            _sendQueue.Enqueue(new GetUserListRequest().GetContainer());

            if (Interlocked.CompareExchange(ref _sending, 1, 0) == 0)
                Send();
        }
        public void GetPrivateMessageList(string sender, string receiver)
        { 
            _sendQueue.Enqueue(new GetPrivateMessageListRequest(sender, receiver).GetContainer());

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