using Messenger.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace Messenger.Network
{
    public class WebSocketClient
    {
        private readonly ConcurrentQueue<MessageContainer> _sendQueue;

        private WebSocket _socket;
        public bool IsConnected
        {
            get
            {
                return _socket?.ReadyState == WebSocketState.Open;
            }
        }

        public event Action Connected;
        public event Action Disconnected;
        public event Action MessageReceived;


        private static void OnMessage(object sender, MessageEventArgs e)
        {
            //if (e.IsText == false)
            //    return;

            //MessageContainer container = JsonConvert.DeserializeObject<MessageContainer>(e.Data);

            //switch (container.Identifier)
            //{
            //   // case nameof()
            //}



            Console.WriteLine(e.Data);
            Console.WriteLine("Enter message:");
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

        public void SendPrivateMessage(User sender, User receiver, string message, DateTime sendTime)
        {
            _socket.Send($"from {sender.Name} to {receiver.Name} ({sendTime}): \n\t\t{message}\n");
        }

        public void SetParams(string ipAddress, int port)
        {
            _socket = new WebSocket($"ws://{ipAddress}:{port}/test");
            
        }
        private void OnOpen(object sender, EventArgs e)
        {
            Connected?.Invoke();
        }
        private void OnClose(object sender, CloseEventArgs e)
        {
            Disconnected?.Invoke();
        }
    }
}
