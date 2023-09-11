using Common;
using System;
using System.Net.Sockets;
using MySql.Data.MySqlClient;
using CSharpServer.ServerTools;

namespace CSharpServer.Servers
{
    //每个客户端自己维护自己的连接
    //每个客户端维护自己的数据库连接
    public class Client
    {
        private Server _server;
        private Message _message;
        private Socket _clientSocket;
        private MySqlConnection? _connection;

        public Client(Socket socket, Server server)
        {
            this._server = server;
            this._message = new Message();
            this._clientSocket = socket;
            this._connection = ConnHelper.Connect();
        }

        #region Public
        public void Send(ActionCode action, string data)
        {
            byte[] bytes = Message.PackData(action, data);
            _clientSocket.Send(bytes);
        }
        #endregion

        #region Private
        private void ReceiveConnect()
        {
            this._clientSocket.BeginReceive(this._message.Data, this._message.TotalCount, this._message.RemainSize, SocketFlags.None, ReceiveCallback, this._clientSocket);
        }
        #endregion
        public void Start()
        {
            ReceiveConnect();
        }

        public void Close()
        {
            //关闭数据库连接对象
            if (_connection != null)
            {
                ConnHelper.CloseConnection(_connection);
                _connection = null;
            }

            //关闭socket连接
            if (this._clientSocket != null && this._clientSocket.Connected)
                this._clientSocket.Close();

            //从server中删除客户端
            this._server.ReomoveClient(this);
        }

        #region 回调
        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                var clientSocket = result.AsyncState as Socket;
                if (clientSocket != null)
                {
                    int count = clientSocket.EndReceive(result);
                    if (count == 0)
                    {
                        Close();
                        LogManager.LogInfo("客户端主动断开连接。");
                        return;
                    }

                    //处理接收到的数据
                    this._message.ReadMessage(count, OnProcessMessage);

                    //处理完数据等待下一次数据发送
                    ReceiveConnect();
                }
            }
            catch (Exception e)
            {
                Close();
                LogManager.LogError(string.Format("接收数据解析错误，关闭客户端连接。 错误信息：{0}，", e));
            }
        }

        private void OnProcessMessage(RequestCode request, ActionCode action, string data)
        {
            _server.HandleRequest(request, action, data, this);
        }
        #endregion
    }
}
