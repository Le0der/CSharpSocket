using Common;
using System;
using System.Diagnostics;
using System.Net.Sockets;

namespace CSharpClient
{
    public class ClientManager
    {
        private const string IP = "127.0.0.1";
        private const int PORT = 6688;

        private Socket? _clientSocket;
        private Message _message = new Message();

        public bool IsReady { get { return _clientSocket != null && _clientSocket.Connected; } }

        public void Init()
        {
            ConnectToServer();
        }

        public void Close()
        {
            ConnectionClose();
        }

        public void SendRequest(RequestCode requestCode, ActionCode actionCode, string data)
        {
            if (IsReady)
            {
                var sendData = Message.PackData(requestCode, actionCode, data);
                _clientSocket.Send(sendData);
            }
        }

        private void ConnectToServer()
        {
            try
            {
                if (IsReady) return;

                _clientSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _clientSocket.Connect(IP, PORT);
                ReceiceData();
            }
            catch (Exception e)
            {
                string errorData = string.Format("Error: 无法连接到服务器，请检查您的网络连接！！ 错误信息：{0}", e);
                throw new Exception(errorData);
            }
        }

        private void ConnectionClose()
        {
            try
            {
                if (IsReady)
                    this._clientSocket?.Close();
            }
            catch (Exception e)
            {
                string errorData = string.Format("Error: 无法关闭跟服务器的连接！！ 错误信息：{0}", e);
                throw new Exception(errorData);
            }
        }

        private void ReceiceData()
        {
            if (IsReady)
                _clientSocket?.BeginReceive(_message.Data, _message.TotalCount, _message.RemainSize, SocketFlags.None, ReceiveDataCallback, null);
        }

        private void ReceiveDataCallback(IAsyncResult result)
        {
            try
            {
                if (IsReady)
                {
                    int count = _clientSocket.EndReceive(result);
                    this._message.ReadMessage(count, OnProcessDataEnd);
                    ReceiceData();
                }
            }
            catch (Exception e)
            {
                string errorData = string.Format("Error: 接收消息错误！！！ 错误信息{0}", e);
                throw new Exception(errorData);
            }
        }

        private void OnProcessDataEnd(ActionCode code, string data)
        {
            Console.WriteLine(string.Format("接收到客户端请求数据， Action: {0}, Datas: {1} 。", code, data));
        }
    }
}
