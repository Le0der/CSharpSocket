using CSharpServer.Servers;
using CSharpServer.ServerTools;

LogManager.LogInfo("WebSocket服务器启动...");

string ip = "127.0.0.1";
int port = 6688;
Server server = new Server(ip, port);

server.Start();

Console.ReadKey();