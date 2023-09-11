using System;
using System.Net;
using System.Net.Sockets;
using Common;
using CSharpServer.Contoller;
using CSharpServer.ServerTools;

namespace CSharpServer.Servers
{
    public class Server
    {
        private Socket? _serverSocket;
        private IPEndPoint? _ipEndPoint;
        private List<Client> _clientList;
        private ControllerManager _controllerManager;

        public Server(string ipStr, int port)
        {
            _clientList = new List<Client>();
            _controllerManager = new ControllerManager(this);

            SetIPAndPort(ipStr, port);
        }

        private void SetIPAndPort(string ipStr, int port)
        {
            _ipEndPoint = new IPEndPoint(IPAddress.Parse(ipStr), port);
        }

        public void Start()
        {
            if (_ipEndPoint != null)
            {
                _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _serverSocket.Bind(_ipEndPoint);
                _serverSocket.Listen(0);
                _serverSocket.BeginAccept(AcceptCallBack, _serverSocket);

                LogManager.LogInfo("服务器启动成功，等待客户端连接...");
            }
        }

        public bool ReomoveClient(Client client)
        {
            lock (_clientList)
            {
                var result = _clientList.Remove(client);
                return result;
            }
        }

        public void SendResponse(Client client, ActionCode action, string data)
        {
            client.Send(action, data);
        }

        public void HandleRequest(RequestCode requets, ActionCode action, string data, Client client)
        {
            _controllerManager.HandleRequest(requets, action, data, client);
        }

        private void AcceptCallBack(IAsyncResult result)
        {
            Socket? serverSocket = result.AsyncState as Socket;
            if (serverSocket != null)
            {
                Socket clientSocket = serverSocket.EndAccept(result);
                Client client = new Client(clientSocket, this);
                client.Start();
                _clientList.Add(client);

                LogManager.LogInfo("客户端连接成功，等待客户端消息...");

                serverSocket.BeginAccept(AcceptCallBack, serverSocket);
            }
        }
    }
}
