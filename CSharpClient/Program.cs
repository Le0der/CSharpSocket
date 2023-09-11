// See https://aka.ms/new-console-template for more information
using CSharpClient;

Console.WriteLine("客户端启动...");

ClientManager clientManager = new ClientManager();

Console.WriteLine();
Console.WriteLine("开始连接服务器...");
clientManager.Init();

Console.WriteLine();
Console.WriteLine("连接服务器成功，等待服务器消息，或发送信息给服务器...");

while (true)
{
    var read = Console.ReadLine();
    if (!string.IsNullOrEmpty(read))
    {
        if (read == "Close")
        {
            clientManager.Close();
            break;
        }
        else
            clientManager.SendRequest(Common.RequestCode.None, Common.ActionCode.None, read);
    }
}

Console.WriteLine("关闭服务器连接，按任意键退出...");
Console.ReadKey();